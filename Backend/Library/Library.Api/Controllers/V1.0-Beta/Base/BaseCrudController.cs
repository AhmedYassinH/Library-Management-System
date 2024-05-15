﻿




using Library.Dal.Exceptions;

namespace Library.Api.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0-Beta")]
    public class BaseCrudController<TEntity, TController, TCreateRequestDto, TUpdateRequestDto, TResponseDto> : ControllerBase
        where TEntity : BaseEntity, new()
        where TController : class
        where TCreateRequestDto : new()
        where TUpdateRequestDto : BaseDTO, new()
        where TResponseDto : BaseDTO, new()
    {
        protected readonly IBaseRepo<TEntity> _mainRepo;
        protected readonly IAppLogging<TController> _logger;
        protected readonly IMapper _mapper;


        protected BaseCrudController(IAppLogging<TController> logger, IBaseRepo<TEntity> mainRepo, IMapper mapper)
        {
            _mainRepo = mainRepo;
            _logger = logger;
            _mapper = mapper;


        }

        /// <summary>
        /// Gets all records
        /// </summary>
        /// <returns>All records</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(401, "Unauthorized access attempted")]
        [SwaggerResponse(403, "Forbidden access attempted")]
        [SwaggerResponse(404, "The requested resource was not found")]
        [SwaggerResponse(500, "An internal server error has occurred")]
        [HttpGet]
        public async virtual Task<ActionResult<IEnumerable<TResponseDto>>> GetAll(
            [FromQuery] string? filterOn, [FromQuery] string? filterQuery, // Filtering
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending,    // Sorting
            [FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1 // Pagination
            )
        {
            IEnumerable<TEntity> entities;
            try
            {
                entities = _mainRepo.GetAllIgnoreQueryFilters(
                    filterOn, filterQuery,
                    sortBy, isAscending ?? true,
                    pageSize, pageNumber
                    );
            }
            catch (FormatException ex)
            {
                throw new customWebExceptions.ValidationException(ex.Message)
                {
                    Code = "BadFilterQueryFormat"
                };
            }
            catch (ArgumentException ex)
            {
                throw new customWebExceptions.ValidationException(ex.Message)
                {
                    Code = "BadFilterOnArgument"
                };
            }


            if (entities == null)
            {
                throw new customWebExceptions.NotFoundException("The requested resource was not found");
            }


            var entityResponseDto = _mapper.Map<IEnumerable<TResponseDto>>(entities);
            return Ok(entityResponseDto);
        }


        /// <summary>
        /// Gets a single record
        /// </summary>
        /// <param name="id">Primary key of the record</param>
        /// <returns>Single record</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(401, "Unauthorized access attempted")]
        [SwaggerResponse(403, "Forbidden access attempted")]
        [SwaggerResponse(404, "The requested resource was not found")]
        [SwaggerResponse(500, "An internal server error has occurred")]
        [HttpGet("{id}")]
        public async virtual Task<ActionResult<TResponseDto>> GetOneAsync(int id)
        {
            var entity = await _mainRepo.FindAsync(id);

            if (entity == null)
            {
                throw new customWebExceptions.NotFoundException("The requested resource was not found");
            }
            return Ok(_mapper.Map<TResponseDto>(entity));
        }




        /// <summary>
        /// Updates a single record
        /// </summary>
        /// <param name="id">Primary key of the record to update</param>
        /// <param name="entity">Entity to update</param>
        /// <returns>Updated record</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(400, "The request was invalid")]
        [SwaggerResponse(401, "Unauthorized access attempted")]
        [SwaggerResponse(403, "Forbidden access attempted")]
        [SwaggerResponse(404, "The requested resource was not found")]
        [SwaggerResponse(500, "An internal server error has occurred")]
        [HttpPut("{id}")]
        public async virtual Task<ActionResult<TResponseDto>> UpdateOneAsync(int id, TUpdateRequestDto entity)
        {

            if (!ModelState.IsValid)
            {

                Dictionary<string, string[]> errors = ModelState.ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(y => y.ErrorMessage).ToArray());

                throw new customWebExceptions.ValidationException(errors);
            }


            if (id != entity.Id)
            {
                _logger.LogAppWarning("Id in the route and the entity do not match");
                throw new customWebExceptions.ConflictException
                    ("Id in the route and the entity do not match");
            }




            TEntity domainEntity = _mapper.Map<TEntity>(entity);

            try
            {

                await _mainRepo.UpdateAsync(domainEntity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new customWebExceptions.WebException(ex.Message)
                {
                    Code = "ConcurrencyError"
                };
            }
            catch (UnknownDatabaseException ex)
            {
                throw new customWebExceptions.WebException(ex.Message)
                {
                    Code = "DatabaseError"
                };
            }




            return Ok(_mapper.Map<TResponseDto>(domainEntity));
        }



        /// <summary>
        /// Adds a single record
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>Added record</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(201, "The execution was successful")]
        [SwaggerResponse(400, "The request was invalid")]
        [SwaggerResponse(401, "Unauthorized access attempted")]
        [SwaggerResponse(403, "Forbidden access attempted")]
        [SwaggerResponse(500, "An internal server error has occurred")]
        [HttpPost]
        public async virtual Task<ActionResult<TResponseDto>> AddOneAsync(TCreateRequestDto entity)
        {


            if (!ModelState.IsValid)
            {
                Dictionary<string, string[]> errors = ModelState.ToDictionary(
                     x => x.Key,
                     x => x.Value.Errors.Select(y => y.ErrorMessage).ToArray());

                throw new customWebExceptions.ValidationException(errors);
            }


            TEntity domainEntity = _mapper.Map<TEntity>(entity);
            try
            {

                await _mainRepo.AddAsync(domainEntity);
            }

            catch (UnknownDatabaseException ex)
            {
                throw new customWebExceptions.WebException(ex.Message)
                {
                    Code = "DatabaseError"
                };
            }

            return CreatedAtAction(nameof(GetOneAsync), new { id = _mapper.Map<TResponseDto>(domainEntity).Id }, _mapper.Map<TResponseDto>(domainEntity));
        }




        /// <summary>
        /// Deletes a single record
        /// </summary>
        /// <remarks>
        /// Sample body:
        /// <pre>
        /// {
        ///   "Id": 1,
        ///   "TimeStamp": "AAAAAAAAB+E="
        /// }
        /// </pre>
        /// </remarks>
        /// <returns>Success</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(400, "The request was invalid")]
        [SwaggerResponse(401, "Unauthorized access attempted")]
        [SwaggerResponse(403, "Forbidden access attempted")]
        [SwaggerResponse(404, "The requested resource was not found")]
        [SwaggerResponse(500, "An internal server error has occurred")]
        [HttpDelete("{id}")]
        public async virtual Task<ActionResult> DeleteOneAsync(int id, BaseDTO entity)
        {

            if (!ModelState.IsValid)
            {
                Dictionary<string, string[]> errors = ModelState.ToDictionary(
                     x => x.Key,
                     x => x.Value.Errors.Select(y => y.ErrorMessage).ToArray());

                throw new customWebExceptions.ValidationException(errors);
            }

            if (id != entity.Id)
            {
                _logger.LogAppWarning("Id in the route and the entity do not match");
                throw new customWebExceptions.ConflictException("Id in the route and the entity do not match");
            }

            TEntity domainEntity = _mapper.Map<TEntity>(entity);
            try
            {

                await _mainRepo.DeleteAsync(domainEntity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new customWebExceptions.WebException(ex.Message)
                {
                    Code = "ConcurrencyError"
                };
            }
            catch (UnknownDatabaseException ex)
            {
                throw new customWebExceptions.WebException(ex.Message)
                {
                    Code = "DatabaseError"
                };
            }


            return Ok();
        }

    }
}
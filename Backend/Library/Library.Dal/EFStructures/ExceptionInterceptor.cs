﻿
namespace Library.Dal.EFStructures
{
    public class ExceptionInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {

            try
            {

                return base.SavingChanges(eventData, result);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("An error occured while interacting with the database.", ex);
            }
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            try
            {

                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("An error occured while interacting with the database.", ex);
            }


        }
    }
}
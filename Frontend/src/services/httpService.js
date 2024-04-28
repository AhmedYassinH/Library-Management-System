import axios from "axios";
import { toast } from "react-hot-toast";
import logger from './logService';


axios.interceptors.response.use(null, error => {
    const expectedError = error.response && error.response.status >= 400 && error.response.status < 500;

    // log(error)
    if (!expectedError) {
        logger.log(error)
        toast.error(error.message)
    }
    // if(expectedError)
    //     toast.error('Expected error occured.')

    return Promise.reject(error);
});


const http = {
    get: axios.get,
    post: axios.post,
    put: axios.put,
    delete: axios.delete,
}

export default http
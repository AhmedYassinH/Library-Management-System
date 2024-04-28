import { useEffect } from 'react';
// import { toast } from 'react-toastify';

import { logout } from '../../services/authService';

const Logout = () => {

    useEffect(() => {
        logout()
        window.location = '/'

    }, []);
    return <h1>logout</h1>;
}

export default Logout;
import axios from 'axios';

export const api = axios.create({
  baseURL: 'http://localhost:5011/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    // Log global errors
    console.error('API Error:', error);
    
    // Check if error is 401/403 (Auth) - though we are local
    if (error.response?.status === 401) {
       console.warn('Unauthorized access');
    }

    return Promise.reject(error);
  }
);

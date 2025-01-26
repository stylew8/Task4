import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: process.env.REACT_APP_API_BASE_URL || 'https://localhost:7257/',
  headers: {
    'Content-Type': 'application/json',
  },
});

const setAuthHeader = (sessionGuid) => {
  if (sessionGuid) {
    axiosInstance.defaults.headers.common['Authorization'] = `${sessionGuid}`;
  } else {
    delete axiosInstance.defaults.headers.common['Authorization'];
  }
};

export const postWithAuth = async (endpoint, data, sessionGuid) => {
  setAuthHeader(sessionGuid);

  try {
    const response = await axiosInstance.post(endpoint, data);
    return response;
  } catch (error) {
    throw error;
  }
};

export const postWithoutAuth = async (endpoint, data) => {
    const response = await axiosInstance.post(endpoint, data);

    return response;
};

export { axiosInstance };

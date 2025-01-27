import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: process.env.REACT_APP_API_BASE_URL || 'https://uniqum.school/api/', 
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

export const getWithAuth = async (endpoint, data, sessionGuid) => {
  setAuthHeader(sessionGuid);

  try {
    const response = await axiosInstance.get(endpoint, data);
    return response;
  } catch (error) {
    throw error;
  }
};

export const patchWithAuth = async (endpoint, data, sessionGuid) => {
  setAuthHeader(sessionGuid);

  try {
    const response = await axiosInstance.patch(endpoint, data);
    return response;
  } catch (error) {
    throw error;
  }
};

export const postWithoutAuth = async (endpoint, data) => {
    const response = await axiosInstance.post(endpoint, data);

    return response;
};

export const deleteWithAuth = async (endpoint, data, sessionGuid) => {
  setAuthHeader(sessionGuid);

  const response = await axiosInstance.delete(endpoint, {data});

  return response;
};

export { axiosInstance };

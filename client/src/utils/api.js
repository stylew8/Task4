import useSWR from 'swr';
import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: process.env.REACT_APP_API_BASE_URL || 'https://localhost:7257/',
  headers: {
    'Content-Type': 'application/json',
  },
});

const setAuthHeader = (sessionGuid) => {
  if (sessionGuid) {
    axiosInstance.defaults.headers.common['Authorization'] = `Bearer ${sessionGuid}`;
  } else {
    delete axiosInstance.defaults.headers.common['Authorization'];
  }
};

const fetcher = async (url) => {
  const response = await axiosInstance.get(url);
  return response.data;
};

export const useFetch = (endpoint, options = {}) => {
  const { data, error, mutate } = useSWR(endpoint, fetcher, options);

  return {
    data,
    error,
    isLoading: !error && !data,
    mutate,
  };
};

export const useAuthFetch = (endpoint, sessionGuid, options = {}) => {
  setAuthHeader(sessionGuid);

  const { data, error, mutate } = useSWR(endpoint, fetcher, options);

  return {
    data,
    error,
    isLoading: !error && !data,
    mutate,
  };
};

export const postWithAuth = async (endpoint, data, sessionGuid) => {
  setAuthHeader(sessionGuid);

  try {
    const response = await axiosInstance.post(endpoint, data);
    return response.data;
  } catch (error) {
    throw error.response?.data || error;
  }
};

export const postWithoutAuth = async (endpoint, data) => {
  try {
    const response = await axiosInstance.post(endpoint, data);
    return response.data;
  } catch (error) {
    throw error.response?.data || error;
  }
};

export { axiosInstance };

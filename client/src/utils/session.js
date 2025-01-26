import Cookies from 'js-cookie';

export const setCookie = (name, value, days = 0, options = {}) => {
    const settings = { path: '/', ...options };

    if (days > 0) {
        settings.expires = days;
    }

    Cookies.set(name, value, settings);
};

export const getCookie = (name) => {
    try {
        const value = Cookies.get(name); 
        if (!value) {
            return null;
        }

        if (value.startsWith("{") || value.startsWith("[")) {
            try {
                return JSON.parse(value);
            } catch (jsonError) {
                return null;
            }
        }

        return value;
    } catch (error) {
        return null;
    }
};

export const deleteCookie = (name) => {
    if (Cookies.get(name)) {
        Cookies.remove(name);
    } else {
    }
};

export const hasCookie = (name) => {
    return !!Cookies.get(name);
};

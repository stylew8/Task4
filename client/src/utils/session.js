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
            console.warn(`Cookie "${name}" not found.`);
            return null;
        }

        if (value.startsWith("{") || value.startsWith("[")) {
            try {
                return JSON.parse(value);
            } catch (jsonError) {
                console.error(`Error parsing cookie "${name}":`, jsonError);
                return null;
            }
        }

        return value;
    } catch (error) {
        console.error(`Error retrieving cookie "${name}":`, error);
        return null;
    }
};

export const deleteCookie = (name) => {
    if (Cookies.get(name)) {
        Cookies.remove(name);
    } else {
        console.warn(`Cookie with name: "${name}" not exsists.`);
    }
};

export const hasCookie = (name) => {
    return !!Cookies.get(name);
};

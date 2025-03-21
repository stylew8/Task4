import React, { useState, useEffect } from "react";
import { Navigate } from "react-router-dom";
import { setCookie, getCookie, deleteCookie, hasCookie } from "../utils/session";
import { postWithAuth, postWithoutAuth } from "../utils/api";

const ProtectedRoute = ({ children, requireAuth }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    const validateSession = async () => {
        const sessionId = getCookie("SessionId");
        const rememberMe = getCookie("RememberMe");

        if (sessionId !== null) {
            try {
                const response = await postWithAuth("auth/validation/session", "", sessionId);
                setIsAuthenticated(true); 
            } catch (error) {
                setIsAuthenticated(false); 
            }
        } else {
            if (rememberMe != null) {
                try {
                    const response2 = await postWithoutAuth("auth/validation/userToken", { userToken: rememberMe });

                    setCookie("SessionId", response2.data.sessionId);
                    setIsAuthenticated(true);
                } catch (error) {
                    setIsAuthenticated(false);
                }
            } else {
                setIsAuthenticated(false);
            }
        }
    };

    validateSession();
}, []); 



  if (requireAuth && !isAuthenticated) {
    return <Navigate to="/login" />;
  }

  if (!requireAuth && isAuthenticated) {
    return <Navigate to="/" />;
  }

  return children;
};

export default ProtectedRoute;
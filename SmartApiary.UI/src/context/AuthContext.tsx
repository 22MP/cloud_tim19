import { createContext, useContext, useState, ReactNode } from "react";

interface AuthUser {
    token: string;
    userId: string;
    email: string;
    role: string;
}

interface AuthContextType {
    user: AuthUser | null;
    login: (user: AuthUser) => void;
    logout: () => void;
    isAdmin: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
    const [user, setUser] = useState<AuthUser | null>(() => {
        const token = localStorage.getItem("token");
        const userId = localStorage.getItem("userId");
        const email = localStorage.getItem("email");
        const role = localStorage.getItem("role");
        if (token && userId && email && role)
            return { token, userId, email, role };
        return null;
    });

    const login = (userData: AuthUser) => {
        localStorage.setItem("token", userData.token);
        localStorage.setItem("userId", userData.userId);
        localStorage.setItem("email", userData.email);
        localStorage.setItem("role", userData.role);
        setUser(userData);
    };

    const logout = () => {
        localStorage.clear();
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{
            user,
            login,
            logout,
            isAdmin: user?.role === "Admin"
        }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error("useAuth must be used within AuthProvider");
    return ctx;
};
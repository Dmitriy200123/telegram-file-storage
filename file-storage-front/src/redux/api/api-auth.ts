import {myHeaders} from "./api";

const baseUrl = "https://localhost:5001/auth";

export const fetchLog = async (url: string) => {
    const response = await fetch(baseUrl + url, {
        headers: myHeaders,
    });
};

export const fetchAuth = async (url: string, config?: any) => {
    const {token, method, body} = config;
    const myHeaders = new Headers({
        'Content-Type': 'application/json',
        'token': token || ""
    });

    const response = await fetch(baseUrl + url, {
        headers: myHeaders,
        method: method ?? "GET",
        body: body ? JSON.stringify(body) : null
    });

    return await response.json();
};





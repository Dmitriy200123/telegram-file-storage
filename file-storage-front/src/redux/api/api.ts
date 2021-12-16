import {TokensType} from "../../models/File";

const baseUrl = "https://localhost:5001/api";

const myHeaders = new Headers({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${localStorage.getItem("token")}`
});

export const fetchLog = async (url: string) => {
    const response = await fetch("https://localhost:5001/auth" + url, {
        headers: myHeaders,
    });
};


export const fetchAuth = async (url: string, token?: string) => {
    const myHeaders = new Headers({
        'Content-Type': 'application/json',
        'token': token || ""
    });

    const response = await fetch("https://localhost:5001/auth" + url, {
        headers: myHeaders,
    });

    return await response.json();
};

export const f1etchIsAuth = async (url: string, body: object) => {

    const bodyJson = JSON.stringify(body);
    const response = await fetch("https://localhost:5001/auth" + url, {
        headers: myHeaders,
        method: "POST",
        body: bodyJson
    });

    return await response.json();
};



export const fetchData = async (url: string) => {
    const response = await fetch(baseUrl + url, {
        headers: myHeaders
    });
    return await response.json();
};

export const fetchConfig = async (url: string, config?: any) => {
    const params = queryParams(config.params);
    const respUrl = baseUrl + url + (params.length > 0 ? "?" + params : "");
    const myHeaders = new Headers({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem("token")}`
    });
    const response = await fetch(respUrl, {
        credentials: "same-origin",
        method: config.method ?? "GET",
        body: JSON.stringify(config.body),
        headers: myHeaders,
    });

    return await response.json();
};


export const fetchConfigText = async (url: string, config?: any) => {
    const params = queryParams(config?.params);
    const respUrl = baseUrl + url + (params?.length > 0 ? "?" + params : "");
    const response = await fetch(respUrl, {
        credentials: "same-origin",
        method: config?.method ?? "GET",
        body: JSON.stringify(config?.body),
        headers: myHeaders
    });
    return await response.text();
};

function queryParams(obj: any) {
    let res = "";
    for (let propName in obj) {
        if (obj[propName] === null || obj[propName] === undefined || obj[propName] === "" || obj[propName]?.length === 0) {
            continue;
        } else if (Array.isArray(obj[propName])) {
            res += `&${propName}=${obj[propName].join(`&${propName}=`)}`
        } else {
            res += `&${propName}=${obj[propName]}`
        }
    }
    return res.slice(1);
}

export type ConfigType<T> = {
    params?: T
};


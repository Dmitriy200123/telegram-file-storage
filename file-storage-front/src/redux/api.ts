const baseUrl = "http://localhost:5001/api";


export const fetchData = async (url: string) => {
    const response = await fetch(baseUrl + url);
    return await response.json();
};


export const fetchConfigData = async (url: string, config?: any) => {
    const params = queryParams(config.params);
    const respUrl = baseUrl + url + (params.length > 0 ? "?" + params : "");
    const response = await fetch(respUrl, {
        method: config.method ?? "GET"
    });
    const res = await response.json()
    return res;
};


export const fetchConfig = async (url: string, config?: any) => {
    const params = queryParams(config.params);
    const respUrl = baseUrl + url + (params.length > 0 ? "?" + params : "");
    const response = await fetch(respUrl, {
        method: config.method ?? "GET"
    });
    return await response.json();
};


export type ConfigType<T> = {
    params?: T
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


function clean(obj: any) {
    for (let propName in obj) {
        if (obj[propName] === null || obj[propName] === undefined || obj[propName]?.length === 0) {
            delete obj[propName];
        }
    }

    return obj;
}

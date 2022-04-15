const baseUrl = process.env.REACT_APP_BACKEND_URL;

export let myHeaders = new Headers({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${localStorage.getItem("jwtToken")}`
});

export function updateAuthToken() {
    myHeaders = new Headers({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem("jwtToken")}`
    });
}

const doRequest = async (url: string, config?: any) => {
    const {params, method, body} = config || {}
    const paramsString = queryParams(params);
    const respUrl = baseUrl + url + (paramsString.length > 0 ? "?" + paramsString : "");
    return await fetch(respUrl, {
        method: method ?? "GET",
        body: JSON.stringify(body),
        headers: myHeaders,
    });
}

export const fetchConfig = async (url: string, config?: any) => {
    const response = await doRequest(url, config);
    if (response.ok)
        return await response.json();
    throw Error();
};

export const fPostFile = async (url: string, body?: any) => {
    let postHeaders = new Headers({
        'Authorization': `Bearer ${localStorage.getItem("jwtToken")}`
    });

    const response = await fetch(baseUrl + url, {
        method: "POST",
        headers: postHeaders,
        body: body,
    });

    if (response.ok)
        return await response.text();
    throw Error();
};

export const fetchConfigText = async (url: string, config?: any) => {
    const response = await doRequest(url, config);
    if (response.ok)
        return await response.text();
    throw Error();
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


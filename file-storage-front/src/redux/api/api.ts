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

export const makeDoRequest = (baseUrl: string) => async (url: string, config?: any) => {
    const {params, method, body} = config || {}
    const paramsString = queryParams(params);
    const respUrl = baseUrl + url + (paramsString.length > 0 ? "?" + paramsString : "");
    return await fetch(respUrl, {
        method: method ?? "GET",
        body: JSON.stringify(body),
        headers: myHeaders,
    });
}

export const makeFetchConfig = (doRequest: (url: string, config?: any) => Promise<Response>) => async (url: string, config?: any) => {
    const response = await doRequest(url, config);
    if (!response.ok)
        throw Error(await response.text());
    return await response.json();
};

export const makeFetchConfigText = (doRequest: (url: string, config?: any) => Promise<Response>) => async (url: string, config?: any) => {
    const response = await doRequest(url, config);
    if (!response.ok)
        throw Error(await response.text());
    return await response.text();
};

function queryParams(obj: { [key: string]: any }) {
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


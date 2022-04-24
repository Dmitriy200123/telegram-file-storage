import {makeDoRequest as doReq, makeFetchConfig, makeFetchConfigText} from "./api";

const baseUrl = "http://localhost:5001";

const doRequest = doReq(baseUrl);
export const fetchConfig = makeFetchConfig(doRequest);
export const fetchConfigText = makeFetchConfigText(doRequest);
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


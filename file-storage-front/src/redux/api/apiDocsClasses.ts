import {makeDoRequest as doReq, makeFetchConfig, makeFetchConfigText} from "./api";

const baseUrl = "http://localhost:5003";

const doRequest = doReq(baseUrl);
export const fetchConfig = makeFetchConfig(doRequest);
export const fetchConfigText = makeFetchConfigText(doRequest);

import {makeDoRequest as doReq, makeFetchConfig, makeFetchConfigText} from "./api";

const baseUrl = process.env.REACT_APP_BACKEND_CLASSIFICATIONS_URL as string;

const doRequest = doReq(baseUrl);
export const fetchConfig = makeFetchConfig(doRequest);
export const fetchConfigText = makeFetchConfigText(doRequest);

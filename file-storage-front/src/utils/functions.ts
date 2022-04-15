import queryString from "querystring";

export function separateStringExtension(line:string) {
    const index = line.lastIndexOf(".");
    if (index >= 0)
        return [line.slice(0, index), line.slice(index)];
    return null;
}

export const AddToUrlQueryParams = (history: any, values: Object) => {
    const urlParams = {};
    Object.keys(values).forEach(key => {
        // @ts-ignore
        const value = values[key];
        if (key === "date" && value) {
            if (value.dateFrom) {
                // @ts-ignore
                urlParams["dateFrom"] = value.dateFrom;
            }
            if (value.dateTo) {
                // @ts-ignore
                urlParams["dateTo"] = value.dateTo;
            }
        } else if (value) {
            if (value instanceof Array) {
                if (value.length > 0) {
                    // @ts-ignore
                    urlParams[key] = value.join(`&`)
                }
            } else {
                // @ts-ignore
                urlParams[key] = value;
            }
        }
    })

    history.push({
        search: queryString.stringify(urlParams),
    })
};

export const GetQueryParamsFromUrl = (history: any) => {
    const urlSearchParams = new URLSearchParams(history.location.search);
    const fileName = urlSearchParams.get("fileName");
    const senderId = urlSearchParams.get("senderIds")?.split("&")?.map((e) => e);
    const categories = urlSearchParams.get("categories")?.split("&")?.map((e) => e);
    const date = {dateFrom: urlSearchParams.get("dateFrom"), dateTo: urlSearchParams.get("dateTo")}
    const chats = urlSearchParams.get("chatIds")?.split("&")?.map((e) => e);
    return {fileName, senderId, categories, date, chats};
}

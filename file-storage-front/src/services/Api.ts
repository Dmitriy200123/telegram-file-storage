import {createApi, fetchBaseQuery} from "@reduxjs/toolkit/query/react";

export const Api = createApi({
    reducerPath: "filesApi",
    baseQuery: fetchBaseQuery({baseUrl: process.env.REACT_APP_BACKEND_URL + "/api"}),
    endpoints: (builder) => ({
        getChats: builder.query({
            query: () => ({
                url: `/chats`
            })
        })
    })
})


import {createApi, fetchBaseQuery} from "@reduxjs/toolkit/query/react";

export const Api = createApi({
    reducerPath: "postApi123",
    baseQuery: fetchBaseQuery({baseUrl: "https://jsonplaceholder.typicode.com"}),
    endpoints: (builder) => ({
        fetchAllPosts: builder.query({
            query: () => ({
                url: `/posts`
            })
        })
    })
})


import {createAsyncThunk} from "@reduxjs/toolkit";

import {fetchData, fetchConfig, fetchConfigText} from "./api/api";

export const fetchIsAuth = createAsyncThunk("profile/isAuth", async (_, thunkAPI) => {
    try {
        const data= await fetchData("/auth/gitlab/refresh");
        return data;
    } catch (e) {
        return thunkAPI.rejectWithValue("Не удалось загрузить чаты");
    }
})


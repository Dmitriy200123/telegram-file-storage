import {createAsyncThunk} from "@reduxjs/toolkit";

import {f1etchIsAuth, fetchAuth, fetchLog} from "./api/api";
import {TokensType} from "../models/File";

export const fetchIsAuth = createAsyncThunk("profile/isAuth", async (_, thunkAPI) => {
    const jwtToken = localStorage.getItem("jwtToken");
    const refreshToken = localStorage.getItem("refreshToken");
    if (jwtToken && refreshToken)
        try {
            const tokens:TokensType = {
                jwtToken: jwtToken,
                refreshToken: refreshToken
            };
            const data:TokensType = await f1etchIsAuth("/gitlab/refresh", tokens);
            return data;
        } catch (e) {
            return thunkAPI.rejectWithValue("Не удалось");
        }
    else
        return thunkAPI.rejectWithValue("Токены неверные");
})

export const fetchAuthGitlab = createAsyncThunk("profile/login", async (token:string, thunkAPI) => {
    try {
        const data:TokensType = await fetchAuth("/gitlab", token);
        return data;
    } catch (e) {
        return thunkAPI.rejectWithValue("Не войти с помощью гитлаба");
    }
})


export const fetchLogout = createAsyncThunk("profile/logout", async (token:string, thunkAPI) => {
    try {
        await fetchLog("/gitlab/logout");
    } catch (e) {
        return thunkAPI.rejectWithValue("Не войти с помощью гитлаба");
    }
})

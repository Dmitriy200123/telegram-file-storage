import {createAsyncThunk} from "@reduxjs/toolkit";

import {TokensType} from "../../models/File";
import {fetchAuth, fetchLog} from "../api/api-auth";
import {fetchConfig, fetchConfigText} from "../api/api";

export const fetchIsAuth = createAsyncThunk("profile/isAuth", async (_, thunkAPI) => {
    const jwtToken = localStorage.getItem("jwtToken");
    const refreshToken = localStorage.getItem("refreshToken");
    if (jwtToken && refreshToken)
        try {
            const tokens:TokensType = {
                jwtToken: jwtToken,
                refreshToken: refreshToken
            };
            const data:TokensType = await fetchAuth("/auth/gitlab/refresh", {body:tokens, method: "POST"});
            return data;
        } catch (e) {
            return thunkAPI.rejectWithValue("Не удалось");
        }
    else
        return thunkAPI.rejectWithValue("Токены неверные");
})

export const fetchAuthGitlab = createAsyncThunk("profile/login", async (token:string, thunkAPI) => {
    try {
        const data:TokensType = await fetchAuth("/auth/gitlab", {token:token});
        return data;
    } catch (e) {
        return thunkAPI.rejectWithValue("Не войти с помощью гитлаба");
    }
})


export const fetchLogout = createAsyncThunk("profile/logout", async (_, thunkAPI) => {
    try {
        await fetchLog("/auth/gitlab/logout");
    } catch (e) {
        return thunkAPI.rejectWithValue("Не войти с помощью гитлаба");
    }
})


export const fetchLogoutTelegram = createAsyncThunk("profile/logout-telegram", async (_, thunkAPI) => {
    try {
        await fetchConfigText("/auth/telegram/logout", {method:"POST"});
    } catch (e) {
        return thunkAPI.rejectWithValue("Не удалось выйти");
    }
})

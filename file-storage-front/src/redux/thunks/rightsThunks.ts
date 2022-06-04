import {createAsyncThunk} from "@reduxjs/toolkit";
import {fetchConfig} from "../api/apiFiles";


export const fetchAllUsers = createAsyncThunk("users-info/users", async (_, thunkAPI) => {
    try {
        return await fetchConfig("/users");
    } catch (e) {
        return thunkAPI.rejectWithValue("Не удалось получить информацию о пользователях");
    }
})


export const fetchUserCurrent = createAsyncThunk("users-info/current", async (_, thunkAPI) => {
    try {
        return await fetchConfig("/users/current");
    } catch (e) {
        return thunkAPI.rejectWithValue("Не удалось получить информацию о текущем пользователе");
    }
})

export const fetchRightsCurrentUser = createAsyncThunk("rights/current", async (_, thunkAPI) => {
    try {
        return await fetchConfig("/rights/currentUserRights");
    } catch (e) {
        return thunkAPI.rejectWithValue("Не удалось получить права текущего пользователя");
    }
})

export const fetchRightsUserById = createAsyncThunk("rights/user", async (id: string, thunkAPI) => {
    try {
        return await fetchConfig(`/rights/userRights?userId=${id}`);
    } catch (e) {
        return thunkAPI.rejectWithValue("Не удалось получить право пользователя");
    }
})

export const fetchRightsDescription = createAsyncThunk("rights/description", async (_, thunkAPI) => {
    try {
        return await fetchConfig("/rights/description");
    } catch (e) {
        return thunkAPI.rejectWithValue("Не удалось получить описание прав");
    }
})


export const postSetRightsUser = createAsyncThunk("rights/set", async (args: { userId: string, grant?: Array<string | number>, revoke?: Array<string | number> }, thunkAPI) => {
    try {
        const data = await fetchConfig("/rights/set", {method: "POST", body: args});
        thunkAPI.dispatch(fetchRightsCurrentUser());
        return data;
    } catch (e) {
        return thunkAPI.rejectWithValue("Не удалось присвоить право пользователю");
    }
})


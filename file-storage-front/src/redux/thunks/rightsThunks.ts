import {createAsyncThunk} from "@reduxjs/toolkit";
import {fetchData} from "../api/api";


export const fetchAllUsers = createAsyncThunk("users-info/users", async (_, thunkAPI) => {
    try {
        const data = await fetchData("/users");
        return data;
    } catch (e) {
        return thunkAPI.rejectWithValue("Не войти с помощью гитлаба");
    }
})


export const fetchUserCurrent = createAsyncThunk("users-info/current", async (_, thunkAPI) => {
    try {
        const data = await fetchData("/users/current");
        return data;
    } catch (e) {
        return thunkAPI.rejectWithValue("Не войти с помощью гитлаба");
    }
})


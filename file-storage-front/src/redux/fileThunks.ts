import {createAsyncThunk} from "@reduxjs/toolkit";
import {fetchConfig, fetchConfigText} from "./api/api";

export const fetchRemoveFile = createAsyncThunk("file/remove", async (id: string, thunkAPI) => {
    try {
        return await fetchConfig(`/files/${id}`, {method: "DELETE"});
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить");
    }
});

export const fetchFile = createAsyncThunk("file/get", async (id: string, thunkAPI) => {
    try {
        return await fetchConfig(`/files/${id}`, {method: "GET"});
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить");
    }
});


export const fetchDownloadLink = async (id: string) => {
    try {
        const link = await fetchConfigText(`/files/${id}/downloadlink`, {method: "GET"});
        window.open(link);
    } catch (err) {

    }
}
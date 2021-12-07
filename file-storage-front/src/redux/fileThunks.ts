import {createAsyncThunk} from "@reduxjs/toolkit";
import {fetchConfig, fetchConfigText} from "./api/api";

export const fetchFile = createAsyncThunk("file/get", async (id: string, thunkAPI) => {
    try {
        return await fetchConfig(`/files/${id}`, {method: "GET"});
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить");
    }
});

export const fetchEditFileName = createAsyncThunk("file/edit", async (args: { id: string, fileName: string }, thunkAPI) => {
    const {id, fileName} = args;
    try {
        await fetchConfigText(`/files/${id}`, {method: "PUT", body: {fileName: fileName}});
        return args;
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось переименовать файл");
    }
});

export const fetchRemoveFile = createAsyncThunk("file/remove", async (id: string, thunkAPI) => {
    try {
        await fetchConfigText(`/files/${id}`, {method: "DELETE"});
        return id;
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось удалить файл");
    }
});


export const fetchDownloadLink = createAsyncThunk("file/download", async (id: string, thunkAPI) => {
    try {
        const link = await fetchConfigText(`/files/${id}/downloadlink`, {method: "GET"});
        window.open(link);
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить файл");
    }
});

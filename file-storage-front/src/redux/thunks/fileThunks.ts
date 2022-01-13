import {createAsyncThunk} from "@reduxjs/toolkit";
import {fetchConfig, fetchConfigText, fPostFile} from "../api/api";

export const fetchFile = createAsyncThunk("file/get", async (id: string, thunkAPI) => {
    try {
        return await fetchConfig(`/api/files/${id}`, {method: "GET"});
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить файл");
    }
});

export const postFile = createAsyncThunk("file/post", async (formData: FormData, thunkAPI) => {
    try {
        return await fPostFile("/api/files", formData);
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить файл");
    }
});

export const postCustomFile =
    createAsyncThunk("file/custom/post", async ({contentType, FileName, message}: { contentType: string, FileName: string, message: string },
                                                                          thunkAPI) => {
    try {
        if (contentType === "4"){
            return await fetchConfigText("/api/files/upload/link", {method: "POST", body: {name: FileName, value:message}});
        } else if (contentType === "5"){
            return await fetchConfigText("/api/files/upload/text", {method: "POST", body: {name: FileName, value:message}});
        }
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить, проверьте вводимые данные");
    }
});

export const fetchEditFileName = createAsyncThunk("file/edit", async (args: { id: string, fileName: string }, thunkAPI) => {
    const {id, fileName} = args;
    try {
        await fetchConfigText(`/api/files/${id}`, {method: "PUT", body: {fileName: fileName}});
        return args;
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось переименовать файл");
    }
});

export const fetchRemoveFile = createAsyncThunk("file/remove", async (id: string, thunkAPI) => {
    try {
        await fetchConfigText(`/api/files/${id}`, {method: "DELETE"});
        return id;
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось удалить файл");
    }
});


export const fetchDownloadLink = createAsyncThunk("file/downloadlink", async (id: string, thunkAPI) => {
    try {
        const link = await fetchConfigText(`/api/files/${id}/downloadlink`, {method: "GET"});
        window.open(link);
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить ссылку на файл");
    }
});

export const fetchFileText = createAsyncThunk("file/readtext", async ({id, type}: {id: string, type: number}, thunkAPI) => {
    try {
        if (type === 5)
            return await fetchConfigText(`/api/files/${id}/text`, {method: "GET"});
        else if (type === 4)
            return await fetchConfigText(`/api/files/${id}/link`, {method: "GET"});
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить текст файла");
    }
});

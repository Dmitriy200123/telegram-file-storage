import {fetchConfig, fetchConfigText, fPostFile} from "../api/apiFiles";
import {AppDispatch} from "../redux-store";
import {MessageTypeEnum} from "../../models/File";
import {profileSlice} from "../profileSlice";
import {editorSlice} from "../editorSlice";
import {filesSlice} from "../filesSlice"

const {addMessage, setLoading: setLoadingProfile} = profileSlice.actions;
const {setFile} = editorSlice.actions;
const {changeFileName, closeModal, removeFile, setLoading, setOpenFile, setFileUrl} = filesSlice.actions;

export const fetchFile = (id: string) => async (dispatch: AppDispatch) => {
    try {
        dispatch(setLoading(true));
        const file = await fetchConfig(`/api/files/${id}`, {method: "GET"});
        dispatch(setOpenFile(file));
        dispatch(setLoading(false));
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
    }
};

export const postFile = (formData: FormData) => async (dispatch: AppDispatch) => {
    try {
        dispatch(setLoading(true));
        await fPostFile("/api/files", formData)
        dispatch(setFile(null));
        dispatch(addMessage({type: MessageTypeEnum.Message, value: "Успешно загружен"}));
        dispatch(setLoading(false));
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
        dispatch(setFile(null));
        dispatch(setLoading(false));
    }
}

export const postCustomFile = ({
                                   contentType,
                                   FileName,
                                   message
                               }: { contentType: string, FileName: string, message: string }) => {
    return async (dispatch: AppDispatch) => {
        try {
            if (contentType === "4") {
                const res = await fetchConfigText("/api/files/upload/link", {
                    method: "POST",
                    body: {name: FileName, value: message}
                });
            } else if (contentType === "5") {
                const res = await fetchConfigText("/api/files/upload/text", {
                    method: "POST",
                    body: {name: FileName, value: message}
                });
            }
            dispatch(addMessage({type: MessageTypeEnum.Message, value: "Файл успешно загружен"}))
        } catch (err) {
            dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
        }
    }
}


export const fetchEditFileName = (args: { id: string, fileName: string }) => async (dispatch: AppDispatch) => {
    const {id, fileName} = args;
    try {
        dispatch(setLoading(true));
        await fetchConfigText(`/api/files/${id}`, {method: "PUT", body: {fileName: fileName}});
        dispatch(changeFileName(args));
        dispatch(addMessage({type: MessageTypeEnum.Message, value: "Успешно изменено имя файла"}));
        dispatch(setLoading(false));
        dispatch(closeModal());
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось переименовать файл"}));
        dispatch(closeModal());
        dispatch(setLoading(false));
    }
}

export const fetchRemoveFile = (id: string) => async (dispatch: AppDispatch) => {
    try {
        dispatch(setLoading(true));
        await fetchConfigText(`/api/files/${id}`, {method: "DELETE"});
        dispatch(removeFile(id));
        dispatch(addMessage({type: MessageTypeEnum.Message, value: "Успешно удален файл"}))
        dispatch(closeModal());
        dispatch(setLoading(false));
    } catch (err) {
        dispatch(setLoading(false));
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось удалить файл"}))
        dispatch(closeModal());
    }
}

export const fetchDownloadLink = (id: string) => async (dispatch: AppDispatch) => {
    try {
        dispatch(setLoading(true))
        const link = await fetchConfigText(`/api/files/${id}/downloadlink`, {method: "GET"});
        window.open(link);
        dispatch(setLoading(false))
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить ссылку на файл"}))
        dispatch(setLoading(false))
    }
}

export const fetchFileText = ({id, type}: { id: string, type: number }) => async (dispatch: AppDispatch) => {
    try {
        dispatch(setLoading(true));
        if (type === 5)
            await fetchConfigText(`/api/files/${id}/text`, {method: "GET"});
        else if (type === 4)
            await fetchConfigText(`/api/files/${id}/link`, {method: "GET"});
        dispatch(addMessage({type: MessageTypeEnum.Message, value: "Успешно загрузилось!"}))
        dispatch(setLoading(false));
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить ссылку на файл"}))
        dispatch(setLoading(false));
    }
}


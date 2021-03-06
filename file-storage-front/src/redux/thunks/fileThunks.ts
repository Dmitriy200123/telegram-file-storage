import {fetchConfig, fetchConfigText, fPostFile} from "../api/apiFiles";
import {AppDispatch} from "../redux-store";
import {MessageTypeEnum} from "../../models/File";
import {profileSlice} from "../profileSlice";
import {editorSlice} from "../editorSlice";
import {filesSlice} from "../filesSlice"

const {addMessage} = profileSlice.actions;
const {setFile} = editorSlice.actions;
const {
    changeFileName,
    closeModal,
    setClassification,
    removeFile,
    setLoading,
    setOpenFile,
    setFileUrl,
    setMessageOpenFile
} = filesSlice.actions;

export const fetchFile = (id: string) => async (dispatch: AppDispatch) => {
    try {
        dispatch(setLoading(true));
        const file = await fetchConfig(`/api/files/${id}`, {method: "GET"});
        dispatch(setOpenFile(file));
        dispatch(setLoading(false));
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось получить файл"}))
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
                await fetchConfigText("/api/files/upload/link", {
                    method: "POST",
                    body: {name: FileName, value: message}
                });
            } else if (contentType === "5") {
                await fetchConfigText("/api/files/upload/text", {
                    method: "POST",
                    body: {name: FileName, value: message}
                });
            }
            dispatch(addMessage({type: MessageTypeEnum.Message, value: "Файл успешно загружен"}))
        } catch (err) {
            let error = err as Error;
            if (error.message === "This is not url") {
                dispatch(addMessage({type: MessageTypeEnum.Error, value: "Проверьте вводимые данные"}))
            } else {
                dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить файл"}))
            }
        }
    }
}


export const fetchEditFileName = (args: { id: string, fileName: string }) => async (dispatch: AppDispatch) => {
    const {id, fileName} = args;
    try {
        dispatch(setLoading(true));
        await fetchConfigText(`/api/files/${id}`, {method: "PUT", body: {fileName: fileName}});
        dispatch(changeFileName(args));
        dispatch(addMessage({type: MessageTypeEnum.Message, value: "Имя файла успешно изменено"}));
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
        dispatch(addMessage({type: MessageTypeEnum.Message, value: "Файл успешно удален"}))
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
        dispatch(setFileUrl({id, url: link}));
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить ссылку на файл"}))
    } finally {
        dispatch(setLoading(false))
    }
}

export const fetchDownloadLinkAndDownloadFile = (id: string) => async (dispatch: AppDispatch) => {
    dispatch(setLoading(true));
    try {
        const link = await fetchConfigText(`/api/files/${id}/downloadlink`, {method: "GET"});
        window.open(link);
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось загрузить ссылку на файл"}))
    } finally {
        dispatch(setLoading(false))
    }
}

export const fetchFileText = ({id, type}: { id: string, type: number }) => async (dispatch: AppDispatch) => {
    try {
        let message = null;
        dispatch(setLoading(true));
        if (type === 5)
            message = await fetchConfigText(`/api/files/${id}/text`, {method: "GET"});
        else if (type === 4)
            message = await fetchConfigText(`/api/files/${id}/link`, {method: "GET"});
        if (message)
            dispatch(setMessageOpenFile(message))
        dispatch(setLoading(false));
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: `Не удалось загрузить ${type === 5 
                ? 'сообщение с пометкой' 
                : 'ссылку'}`}));
        dispatch(setLoading(false));
    }
}

export const patchAssignClassification = ({
                                              documentId,
                                              classId
                                          }: { documentId: string, classId: string }) => async (dispatch: AppDispatch) => {
    try {
        dispatch(setLoading(true));
        const response = await fetchConfig(`/api/files/documents/${documentId}/assign-classification`,
            {method: "PATCH", body: classId});
        dispatch(setClassification({fileId: documentId, classification: response.classification}))
        dispatch(closeModal());
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось присвоить классификацию документу"}))
    } finally {
        dispatch(setLoading(false));
    }
}

export const deleteClassificationDocument = ({
                                                 documentId,
                                                 classId
                                             }: { documentId: string, classId: string }) => async (dispatch: AppDispatch) => {
    try {
        dispatch(setLoading(true));
        await fetchConfigText(`/api/files/documents/${documentId}/revoke-classification`,
            {method: "PATCH", body: classId});
        dispatch(setClassification({fileId: documentId, classification: null}))
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось отозвать классификацию у документа"}))
    } finally {
        dispatch(setLoading(false));
    }
}
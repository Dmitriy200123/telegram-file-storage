import {fetchConfig, fetchConfigText} from "../api/apiDocsClasses";
import {AppDispatch} from "../redux-store";
import {classesDocsSlice} from "./classesDocsSlice";
import {ClassificationType} from "../../models/Classification";
import {filesSliceActions} from "../filesSlice";
import {profileSlice} from "../profileSlice";
import {MessageTypeEnum} from "../../models/File";

const {setCount, setClassifications, renameClassification, deleteClassification, setClassification, closeModal,
removeClassificationTag, addClassificationTag, setIsFetchClassifications} = classesDocsSlice.actions;
const {setLoading} = filesSliceActions;
const {addMessage} = profileSlice.actions;

export const fetchCountClassifications = (query?: string) => async (dispatch: AppDispatch) => {
    dispatch(setLoading(true));
    try {
        const count = +await fetchConfigText(`/api/documentClassifications/count`, {method: "GET", params: {query}});
        dispatch(setCount(count));
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось получить количество классификаций"}))
    } finally {
        dispatch(setLoading(false));
    }
};

type FetchClassificationsType = {
    skip: number,
    take: number,
    query?: string,
    includeClassificationWords?: boolean
}

export const fetchClassifications = (args: FetchClassificationsType) => async (dispatch: AppDispatch) => {
    dispatch(setLoading(true));
    if (!args.includeClassificationWords) {
        args.includeClassificationWords = true;
    }

    try {
        const classifications: ClassificationType[] = await fetchConfig(`/api/documentClassifications`, {
            method: "GET",
            params: args
        });
        dispatch(setClassifications(classifications));
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось получить классификации"}))
    } finally {
        dispatch(setLoading(false));
    }
};

export const fetchAllClassifications = (query?:string) => async (dispatch: AppDispatch) => {
    dispatch(setLoading(true));
    const params: FetchClassificationsType = {
        skip: 0, take: 30, includeClassificationWords: false, query: query || ""
    }

    try {
        const array: ClassificationType[] = [];
        do {
            const classifications: ClassificationType[] = await fetchConfig(`/api/documentClassifications`, {
                method: "GET",
                params: params
            });
            if (classifications.length === 0) {
                params.take = -1;
            }
            else {
                params.skip+= params.take;
                array.push(...classifications);
            }
        } while (params.take !== -1);
        dispatch(setClassifications(array));
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось получить классификации"}))
    } finally {
        dispatch(setLoading(false));
    }
};

export const fetchClassification = (id:string ) => async (dispatch: AppDispatch) => {
    dispatch(setLoading(true));

    try {
        const classification: ClassificationType = await fetchConfig(`/api/documentClassifications/${id}`, {
            method: "GET",
            params: {includeClassificationWords: true}
        });

        dispatch(setClassification(classification));
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось получить классификацию"}))
    } finally {
        dispatch(setLoading(false));
    }
};


export const fetchRenameClassification = ({
                                              id,
                                              name
                                          }: { id: string, name: string }) => async (dispatch: AppDispatch) => {
    dispatch(setLoading(true));
    try {
        await fetchConfigText(`/api/documentClassifications/${id}`, {
            method: "PUT",
            body: name
        });
        dispatch(renameClassification({id, name}));
        dispatch(closeModal());
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось переименовать классификацию"}))
    } finally {
        dispatch(setLoading(false));
    }
};

export const fetchDeleteClassification = ({id}: { id: string }) => async (dispatch: AppDispatch) => {
    dispatch(setLoading(true));
    try {
        await fetchConfigText(`/api/documentClassifications/${id}`, {
            method: "DELETE",
        });
        dispatch(deleteClassification({id}));
        dispatch(setIsFetchClassifications(true));
        dispatch(closeModal());
        dispatch(addMessage({type: MessageTypeEnum.Message, value: "Классификация успешно удалена"}));
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось удалить классификацию"}))
    } finally {
        dispatch(setLoading(false));
    }
};

type PostClassType = { "name": string, "classificationWords": { "value": string }[] }

export const postAddClassification = (classification: PostClassType) => async (dispatch: AppDispatch) => {
    dispatch(setLoading(true));
    try {
        await fetchConfigText(`/api/documentClassifications`, {
            method: "POST",
            body: classification
        });

        // @ts-ignore
        dispatch(setClassifications(classification))
        dispatch(setIsFetchClassifications(true));
        dispatch(closeModal())
        dispatch(addMessage({type: MessageTypeEnum.Message, value: "Классификация успешно загружена"}));
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось добавить классификацию"}))
    } finally {
        dispatch(setLoading(false));
    }
};


export const postAddToClassificationWord = (args: { classId: string, value: string }) => async (dispatch: AppDispatch) => {
    dispatch(setLoading(true));
    try {
        const id = (await fetchConfig(`/api/documentClassifications/${args.classId}/words`, {
            method: "POST",
            body: {value: args.value}
        }))
            //.replace(/^.|.$/g,"");
        dispatch(addClassificationTag({...args, tagId: id}))
        return "error";
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось добавить слово в классификацию"}))
    } finally {
        dispatch(setLoading(false));
    }
};

export const fetchDeleteToClassificationWord = (args: { classId: string, tagId: string}) => async (dispatch: AppDispatch) => {
    dispatch(setLoading(true));
    try {
        await fetchConfigText(`/api/documentClassifications/words/${args.tagId}`, {
            method: "DELETE",
        });
        dispatch(removeClassificationTag(args));
    } catch (err) {
        dispatch(addMessage({type: MessageTypeEnum.Error, value: "Не удалось удалить слово из классификации"}))
    } finally {
        dispatch(setLoading(false));
    }
};

import React, {useEffect} from 'react';
import "./FilesMain.scss"
import Paginator from '../utils/Paginator/Paginator';
import FragmentFile from "./FragmentFile";
import {useHistory} from "react-router-dom";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {SubmitHandler, useForm} from "react-hook-form";
import {fetchFiles, fetchFilters} from "../../redux/thunks/mainThunks";
import {filesSlice} from "../../redux/filesSlice";
import {AddToUrlQueryParams, GetQueryParamsFromUrl} from "../../utils/functions";
import {Filters} from "./Filters";

let isCurrentPageChanged = false;
const {changePaginatorPage} = filesSlice.actions;

const FilesMain = () => {
    const rights = useAppSelector((state) => state.profile.rights);

    const filesData = useAppSelector((state) => state.filesReducer.files);
    const filesTypes = useAppSelector((state) => state.filesReducer.filesTypes);

    const paginator = useAppSelector((state) => state.filesReducer.paginator);
    const currentPage = useAppSelector((state) => state.filesReducer.paginator.currentPage);
    const filesInPage = useAppSelector((state) => state.filesReducer.paginator.filesInPage);

    const dispatch = useAppDispatch();
    const history = useHistory();

    useEffect(() => {
        const {fileName, chats, senderId, categories, date} = GetQueryParamsFromUrl(history);
        dispatch(fetchFilters());
        setValue("fileName", fileName);
        setValue("senderIds", senderId);
        setValue("categories", categories);
        setValue("chatIds", chats);
        setValue("date", date);
    }, []);

    useEffect(() => {
        isCurrentPageChanged = true;
        onChangeForm();
    }, [currentPage])


    const {handleSubmit, formState: {errors}, setValue, getValues} = useForm<TypeSelectFilters>();
    const dispatchValuesForm: SubmitHandler<TypeSelectFilters> = (formData) => {
        AddToUrlQueryParams(history, formData);
        const form = {
            take: filesInPage,
            fileName: formData.fileName,
            senderIds: formData.senderIds, categories: formData.categories,
            dateTo: formData.date?.dateTo,
            dateFrom: formData.date?.dateFrom,
            chatIds: formData.chatIds,
        };

        if (isCurrentPageChanged) {
            dispatch(fetchFiles({
                skip: currentPage > 0 ? (currentPage - 1) * filesInPage : 0,
                ...form
            }));
        } else {
            if (currentPage !== 1)
                dispatch(changePaginatorPage(1));
            else
                dispatch(fetchFiles({
                    skip: 0, ...form
                }));
        }

        isCurrentPageChanged = false;
    };

    const FragmentsFiles = filesData.map((f) => <FragmentFile key={f.fileId} file={f} rights={rights || []}
                                                              types={filesTypes}/>);

    const onChangeForm = handleSubmit(dispatchValuesForm);
    const setValueForm = (name: string, value: any) => {
        setValue(name as "fileName" | "senderIds" | "date" | "chatIds" | "categories", value, {
            shouldValidate: true,
            shouldDirty: true
        });
    }

    return (
        <div className={"files-main"}>
            <h2 className={"files-main__title"}>Файлы</h2>
            <div className={"files-main__content"}>
                <form className={"files"} onSubmit={onChangeForm}>
                    <h3 className={"files__title"}>Название</h3>
                    <h3 className={"files__title"}>Дата</h3>
                    <h3 className={"files__title"}>Формат</h3>
                    <h3 className={"files__title"}>Отправитель</h3>
                    <h3 className={"files__title"}>Чаты</h3>
                    <Filters setValueForm={setValueForm} getValues={getValues}/>
                    {FragmentsFiles}
                </form>
            </div>
            <Paginator paginator={paginator}/>
        </div>
    );
};


export type TypeSelectFilters = {
    fileName: string | undefined | null,
    senderIds: string[] | undefined | null,
    date: { dateFrom: string | null, dateTo: string | null },
    chatIds: string[] | undefined | null,
    categories: string[] | undefined | null,
}

export default FilesMain;

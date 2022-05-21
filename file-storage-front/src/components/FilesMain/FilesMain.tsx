import React, {useEffect} from 'react';
import PaginatorNeNorm from '../utils/Paginator/PaginatorNeNorm';
import FragmentFile from "./FragmentFile";
import {useHistory} from "react-router-dom";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {SubmitHandler, useForm} from "react-hook-form";
import {fetchDocuments, fetchFiles, fetchFilters} from "../../redux/thunks/mainThunks";
import {AddToUrlQueryParams, GetQueryParamsFromUrl} from "../../utils/functions";
import {Filters} from "./Filters/Filters";
import {fetchAllClassifications} from "../../redux/classesDocs/classesDocsThunks";
import classNames from "classnames";
import "./FilesMain.scss"

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
        const {fileName, chats, senderId, categories, date, classificationIds} = GetQueryParamsFromUrl(history);
        dispatch(fetchFilters());
        dispatch(fetchAllClassifications());
        setValue("fileName", fileName);
        setValue("senderIds", senderId);
        setValue("categories", categories);
        setValue("chatIds", chats);
        setValue("date", date);
        setValue("classificationIds", classificationIds);
    }, []);

    useEffect(() => {
        onChangeForm();
    }, [currentPage])


    const {handleSubmit, formState: {errors}, setValue, getValues, reset} = useForm<TypeSelectFilters>();
    const dispatchValuesForm: SubmitHandler<TypeSelectFilters> = (formData) => {
        AddToUrlQueryParams(history, formData);
        const form = {
            take: filesInPage,
            senderIds: formData.senderIds, categories: formData.categories,
            dateTo: formData.date?.dateTo,
            dateFrom: formData.date?.dateFrom,
            chatIds: formData.chatIds,
        };
        if (formData.categories && formData.categories.length === 1 && +formData.categories[0] === 6) {
            dispatch(fetchDocuments({
                skip: currentPage > 0 ? (currentPage - 1) * filesInPage : 0,
                ...form,
                phrase: formData.fileName,
                classificationIds: formData.classificationIds
            }));
        } else {
            dispatch(fetchFiles({
                skip: currentPage > 0 ? (currentPage - 1) * filesInPage : 0,
                ...form,
                fileName: formData.fileName
            }));
        }

    };

    const onChangeForm = handleSubmit(dispatchValuesForm);

    const FragmentsFiles = filesData.map((f) => <FragmentFile key={f.fileId} file={f} rights={rights || []}
                                                              types={filesTypes} fetchFiles={onChangeForm}/>);

    const setValueForm = (name: string, value: any) => {
        setValue(name as "fileName" | "senderIds" | "date" | "chatIds" | "categories", value, {
            shouldValidate: true
        });
    }
    return (
        <div className={"files-main"}>
            <h2 className={"files-main__title"}>Поиск файлов</h2>
            <div className={"files-main__content"}>
                <form className={"files"} onSubmit={onChangeForm}>
                    <Filters setValueForm={setValueForm} getValues={getValues} reset={reset}/>
                    <div className={"files__files"}>
                        <section className={"files__itemsHead"}>
                            <div className={classNames("files__item", "files__item_title")}>Название</div>
                            <div className={classNames("files__item", "files__item_title")}>Дата</div>
                            <div className={classNames("files__item", "files__item_title")}>Тип</div>
                            <div className={classNames("files__item", "files__item_title")}>Отправитель</div>
                            <div className={classNames("files__item", "files__item_title", "files__item_relative")}>
                                Чат
                            </div>
                        </section>
                        {FragmentsFiles}
                    </div>
                </form>
            </div>
            <PaginatorNeNorm paginator={paginator}/>
        </div>
    );
};


export type TypeSelectFilters = {
    fileName: string | undefined | null,
    senderIds: string[] | undefined | null,
    date: { dateFrom: string | null, dateTo: string | null },
    chatIds: string[] | undefined | null,
    categories: string[] | undefined | null,
    classificationIds?: string[] | null
}

export default FilesMain;

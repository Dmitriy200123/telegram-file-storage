import React, {FC, useEffect} from 'react';
import "./FilesMain.scss"
import Paginator from '../utils/Paginator/Paginator';
import FragmentFile from "./FragmentFile";
import {useHistory} from "react-router-dom";
import * as queryString from "querystring";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {configFilters} from "./ConfigFilters";
import {SubmitHandler, useForm} from "react-hook-form";
import {Select} from "../utils/Inputs/Select";
import {fetchFiles, fetchFilters} from "../../redux/thunks/mainThunks";
import {ReactComponent as Search} from "./../../assets/search.svg";
import {filesSlice} from "../../redux/filesSlice";
import {SelectTime} from "../utils/Inputs/SelectDate";
import {AddToUrlQueryParams, GetQueryParamsFromUrl} from "../../utils/functions";

let isCurrentPageChanged = false;
const {changePaginatorPage} = filesSlice.actions;

const FilesMain = () => {
    const state = useAppSelector((state) => state);
    const {filesReducer, profile} = state;
    const {rights} = profile;
    const {files: filesData, filesNames, chats, senders, paginator, filesTypes} = filesReducer;
    const {currentPage, filesInPage} = paginator;
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

    const {optionsName, optionsSender, optionsChat} = configFilters(filesNames, chats, senders);
    const optionsCategory = filesTypes && Object.keys(filesTypes).map((key) => ({label: filesTypes[key], value: key}));
    const {register, handleSubmit, formState: {errors}, setValue, getValues} = useForm();
    const dispatchValuesForm: SubmitHandler<any> = (formData) => {
        AddToUrlQueryParams(history, formData);
        let form = {
            take: filesInPage,
            fileName: formData.fileName,
            senderIds: formData.senderIds, categories: formData.categories,
            dateTo: formData.date?.dateTo,
            dateFrom: formData.date?.dateFrom,
            chatIds: formData.chatIds,
        };
        console.log(form);
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
    const setValueForm = (name: any, value: any) => {
        setValue(name, value, {
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
                    <Select name={"fileName"} className={"files__filter files__filter_select"} register={register}
                            setValue={setValueForm}
                            values={getValues("fileName")} options={optionsName} isMulti={false}/>
                    <SelectTime name={"date"} className={"files__filter files__filter_select"} register={register}
                               setValue={setValueForm}
                                values={getValues("date")} placeholder={"Выберите дату"}/>
                    <Select name={"categories"} className={"files__filter files__filter_select"} register={register}
                             setValue={setValueForm}
                            values={getValues("categories")} options={optionsCategory} isMulti={true}/>
                    <Select name={"senderIds"} className={"files__filter files__filter_select"} register={register}
                            setValue={setValueForm}
                            values={getValues("senderIds")} options={optionsSender} isMulti={true}/>
                    <div className={"files__filter files__filter_last files__filter_select files__filter_search"}>
                        <Select name={"chatIds"}
                                register={register}
                                setValue={setValueForm}
                                values={getValues("chatIds")} options={optionsChat} isMulti={true}/>
                        <button>
                            <Search/>
                        </button>
                    </div>
                    {FragmentsFiles}
                </form>
            </div>
            <Paginator paginator={paginator}/>
        </div>
    );
};



export default FilesMain;

import React, {useEffect} from 'react';
import "./FilesMain.scss"
import Paginator from '../utils/Paginator/Paginator';
import FragmentFile from "./FragmentFile";
import {useHistory} from "react-router-dom";
import * as queryString from "querystring";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {configFilters} from "./ConfigFilters";
import {SubmitHandler, useForm} from "react-hook-form";
import {Select} from "../utils/Inputs/Select";
import {fetchFiles, fetchFilters} from "../../redux/actionsCreators";
import {Category} from "../../models/File";
import {SelectTime} from "../utils/Inputs/SelectDate";

const FilesMain = () => {
    const filesReducer = useAppSelector((state) => state.filesReducer);
    const filesData = filesReducer.files;
    const chats = filesReducer.chats;
    const senders = filesReducer.senders;
    const dispatch = useAppDispatch();
    const history = useHistory();

    useEffect(() => {
        dispatch(fetchFilters());
        // dispatch(fetchFiles({skip:0, take: 5}));
        const {fileName, chats, senderId, categories, date} = GetQueryParamsFromUrl(history);
        console.log(chats);
        setValue("fileName", fileName);
        setValue("sendersIds", senderId);
        setValue("categories", categories);
        setValue("chatIds", chats);
        setValue("date", date);
    }, []);

    const {optionsName, optionsCategory, optionsSender, optionsChat} = configFilters(filesData, chats, senders);

    const {register, handleSubmit, formState: {errors}, setValue, getValues} = useForm();
    const dispatchValuesForm: SubmitHandler<any> = (formData) => {
        AddToUrlQueryParams(history, formData);
        dispatch(fetchFiles({skip:0, take: 5, ...formData}));

        //todo: thunk request files width formData
    };

    const FragmentsFiles = filesData.map((f) => <FragmentFile key={f.fileId} {...f}/>);

    const onChangeForm = handleSubmit(dispatchValuesForm);
    return (
        <div className={"files-main"}>
            <h2 className={"files-main__title"}>Файлы</h2>
            <div className={"files-main__content"}>
                <form className={"files"}>
                    <h3 className={"files__title"}>Название</h3>
                    <h3 className={"files__title"}>Дата</h3>
                    <h3 className={"files__title"}>Формат</h3>
                    <h3 className={"files__title"}>Отправитель</h3>
                    <h3 className={"files__title"}>Чаты</h3>
                    <Select name={"fileName"} className={"files__filter files__filter_select"} register={register}
                            onChangeForm={onChangeForm} setValue={setValue}
                            values={getValues("fileName")} options={optionsName} isMulti={true}/>
                    <SelectTime name={"date"} className={"files__filter files__filter_select"} register={register}
                                onChangeForm={onChangeForm} setValue={setValue}
                                values={getValues("date")} placeholder={"Выберите дату"}/>
                    <Select name={"categories"} className={"files__filter files__filter_select"} register={register}
                            onChangeForm={onChangeForm} setValue={setValue}
                            values={getValues("categories")} options={optionsCategory} isMulti={true}/>
                    <Select name={"sendersIds"} className={"files__filter files__filter_select"} register={register}
                            onChangeForm={onChangeForm} setValue={setValue}
                            values={getValues("sendersIds")} options={optionsSender} isMulti={true}/>
                    <Select name={"chatIds"} className={"files__filter files__filter_last files__filter_select"}
                            register={register}
                            onChangeForm={onChangeForm} setValue={setValue}
                            values={getValues("chatIds")} options={optionsChat} isMulti={true}/>
                    {FragmentsFiles}
                </form>
            </div>
            <Paginator count={filesData.length}/>
        </div>
    );
};


//#region utils
const AddToUrlQueryParams = (history: any, values: Object) => {
    const urlParams = {};
    Object.keys(values).forEach(key => {
        // @ts-ignore
        const value = values[key];
        if (value) {
            if (value instanceof Array) {
                if (value.length > 0) {
                    // @ts-ignore
                    urlParams[key] = value.join(`&`)
                }
            } else {
                // @ts-ignore
                urlParams[key] = value;
            }
        }
    })

    history.push({
        search: queryString.stringify(urlParams),
    })
};

const GetQueryParamsFromUrl = (history: any) => {
    const urlSearchParams = new URLSearchParams(history.location.search);
    const fileName = urlSearchParams.get("fileName");
    const senderId = urlSearchParams.get("sendersIds")?.split("&")?.map((e) => e);
    const categories = urlSearchParams.get("categories")?.split("&") as any;
    const date = urlSearchParams.get("date");
    const chats = urlSearchParams.get("chatIds")?.split("&")?.map((e) => e);
    return {fileName, senderId, categories, date, chats};
}
//#endregion

export default FilesMain;

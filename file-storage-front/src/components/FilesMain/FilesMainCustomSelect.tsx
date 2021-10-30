import React, {useEffect, useState} from 'react';
import "./FilesMain.scss"
import Paginator from '../utils/Paginator/Paginator';
import FragmentFile from "./FragmentFile";
import {useHistory} from "react-router-dom";
import * as queryString from "querystring";
import {filesSlice} from "../../redux/filesSlice";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {configFilters} from "./ConfigFilters";
import Select from "../utils/Inputs/Select";
import {useForm} from "react-hook-form";

const actions = filesSlice.actions;
const FilesMainCustomSelect = () => {
    const filesReducer = useAppSelector((state) => state.filesReducer);
    const filesData = filesReducer.files;
    const form = filesReducer.form;

    const dispatch = useAppDispatch();
    const history = useHistory();
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        setLoading(true);
        const urlSearchParams = new URLSearchParams(history.location.search);
        //todo: read array from url
        //#region todo: Instead of this do thunk request to api
        let fileName = urlSearchParams.get("fileName")?.split("&");
        let senderId = urlSearchParams.get("senders")?.split("&")?.map((e) => +e);
        let categories = urlSearchParams.get("categories")?.split("&") as any;
        let date = urlSearchParams.get("date");
        let chats = urlSearchParams.get("chats")?.split("&")?.map((e) => +e);
        dispatch(actions.changeFilters({
            fileName: fileName,
            senders: senderId,
            categories:categories,
            date:date,
            chats:chats,
        }));

        setValue("fileName", fileName);
        setValue("senders", senderId);
        setValue("categories", categories);
        setValue("chats", chats);
        setValue("date", date);
        console.log(chats)
        //#endregion
        setLoading(false);
    }, [])

    useEffect(() => {
        if (loading)
            return;
        const urlParams = {};
        Object.keys(form).forEach(key => {
            // @ts-ignore
            const value = form[key];
            if (value) {
                if (value instanceof Array ) {
                    if (value.length > 0) {
                        // @ts-ignore
                        urlParams[key] = value.join(`&`)
                    }
                }
                else {
                    // @ts-ignore
                    urlParams[key] = value;
                }
            }
        })
        console.log(urlParams)

        history.push({
            search: queryString.stringify(urlParams)
        })
    }, [form])

    const {optionsName, optionsCategory, optionsSender, optionsChat, optionsDate} = configFilters(filesData);


    const FragmentsFiles = filesData.map((f) => <FragmentFile fileType={f.fileType} fileId={f.fileId}
                                                              fileName={f.fileName} chatId={f.chatId}
                                                              senderId={f.senderId} uploadDate={f.uploadDate}/>);
    const {register, handleSubmit, formState: {errors, dirtyFields}, setValue, getValues} = useForm();
    const consoleLog = (e: any) => {
        dispatch(actions.changeFilters(e));
    }

    const onChangeForm = handleSubmit(consoleLog);
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
                    <Select name={"fileName"} className={"files__filter files__filter_select"} register={register}  onChangeForm={onChangeForm} setValue={setValue}
                            getValues={getValues} options={optionsName}/>
                    <Select name={"date"} className={"files__filter files__filter_select"} register={register}  onChangeForm={onChangeForm} setValue={setValue}
                            getValues={getValues} options={optionsDate}/>
                    <Select name={"categories"} className={"files__filter files__filter_select"} register={register}  onChangeForm={onChangeForm} setValue={setValue}
                            getValues={getValues} options={optionsCategory}/>
                    <Select name={"senders"} className={"files__filter files__filter_select"} register={register}  onChangeForm={onChangeForm} setValue={setValue}
                            getValues={getValues} options={optionsSender}/>
                    <Select name={"chats"} className={"files__filter files__filter_select"} register={register}  onChangeForm={onChangeForm} setValue={setValue}
                            getValues={getValues} options={optionsChat}/>
                    {FragmentsFiles}
                </form>
            </div>
            <Paginator count={filesData.length}/>
        </div>
    );
}


export default FilesMainCustomSelect;

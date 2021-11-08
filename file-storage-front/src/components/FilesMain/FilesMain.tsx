import React, {useEffect, useState} from 'react';
import "./FilesMain.scss"
import Paginator from '../utils/Paginator/Paginator';
import FragmentFile from "./FragmentFile";
import {useHistory} from "react-router-dom";
import * as queryString from "querystring";
import {filesSlice} from "../../redux/filesSlice";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {configFilters} from "./ConfigFilters";
import {SubmitHandler, useForm} from "react-hook-form";
import {Select} from "../utils/Inputs/Select";

const actions = filesSlice.actions;
const FilesMain = () => {
    const filesReducer = useAppSelector((state) => state.filesReducer);
    const filesData = filesReducer.files;
    const form = filesReducer.form;

    const dispatch = useAppDispatch();
    const history = useHistory();
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        setLoading(true);
        const urlSearchParams = new URLSearchParams(history.location.search);
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

        history.push({
            search: queryString.stringify(urlParams)
        })
    }, [form])

    const {optionsName, optionsCategory, optionsSender, optionsChat, optionsDate} = configFilters(filesData);



    const {register, handleSubmit, formState: {errors}, setValue, getValues} = useForm();
    const dispatchValuesForm: SubmitHandler<any> = (e) => {
        dispatch(actions.changeFilters(e));
    }
    const onChangeForm = handleSubmit(dispatchValuesForm);
    const FragmentsFiles = filesData.map((f) => <FragmentFile fileType={f.fileType} fileId={f.fileId}
                                                              fileName={f.fileName} chatId={f.chatId}
                                                              senderId={f.senderId} uploadDate={f.uploadDate}/>);
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
                            getValues={getValues} options={optionsName} isMulti={true}/>
                    <Select name={"date"} className={"files__filter files__filter_select"} register={register}  onChangeForm={onChangeForm} setValue={setValue}
                            getValues={getValues} options={optionsDate} placeholder={"Выберите дату"}/>
                    <Select name={"categories"} className={"files__filter files__filter_select"} register={register}  onChangeForm={onChangeForm} setValue={setValue}
                            getValues={getValues} options={optionsCategory} isMulti={true}/>
                    <Select name={"senders"} className={"files__filter files__filter_select"} register={register}  onChangeForm={onChangeForm} setValue={setValue}
                            getValues={getValues} options={optionsSender} isMulti={true}/>
                    <Select name={"chats"} className={"files__filter files__filter_select"} register={register}  onChangeForm={onChangeForm} setValue={setValue}
                            getValues={getValues} options={optionsChat} isMulti={true}/>
                    {FragmentsFiles}
                </form>
            </div>
            <Paginator count={filesData.length}/>
        </div>
    );
}


export default FilesMain;

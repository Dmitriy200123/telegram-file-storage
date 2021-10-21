import React, {useEffect, useState} from 'react';
import "./FilesMain.scss"
import Select, {MultiValue, SingleValue} from "react-select";
import Paginator from '../utils/Paginator/Paginator';
import {useDispatch, useSelector} from "react-redux";
import {AppStateType} from "../../redux/redux-store";
import {Category} from "../../types/types";
import {actions, FormType} from "../../redux/files-reducer";
import FragmentFile from "./FragmentFile";
import {useHistory} from "react-router-dom";
import * as queryString from "querystring";

const optionsCategory: Array<{ value: Category, label: Category }> = [
    {value: 'images', label: 'images'},
    {value: 'links', label: 'links'},
    {value: 'video', label: 'video'},
    {value: 'documents', label: 'documents'},
];
const optionsDate = [
    {value: 'За все время', label: 'За все время'},
    {value: 'Сегодня', label: 'Сегодня'},
    {value: 'Вчера', label: 'Вчера'},
    {value: 'За последние 7 дней', label: 'За последние 7 дней'},
    {value: 'За последние 30 дней', label: 'За последние 30 дней'},
    {value: '', label: 'Другой период...'}
];

const select = {
    input: (asd: any) => ({
        ...asd,
        height: 30,
    }),
};

const FilesMain = () => {
    const filesReducer = useSelector((state: AppStateType) => state.filesReducer);
    const dispatch = useDispatch();
    const filesData = filesReducer.files;
    const form = filesReducer.form;
    const history = useHistory();
    const [loading, setLoading] = useState(true);
    useEffect(() => {
        setLoading(true);
        let urlSearchParams = new URLSearchParams(history.location.search);
        //todo: read array from url
        //#region todo: Instead of this do thunk request to api
        dispatch(actions.changeFilterFileName(urlSearchParams.get("fileName")));
        dispatch(actions.changeFilterSenders(urlSearchParams.get("senderId") as any));
        dispatch(actions.changeFilterCategories(urlSearchParams.get("categories") as any));
        dispatch(actions.changeFilterDate(urlSearchParams.get("date")));
        dispatch(actions.changeFilterChats(urlSearchParams.get("chats") as any));
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
                // @ts-ignore
                urlParams[key] = value instanceof Array ? value.join("&") : (value);
            }
        })
        history.push({
            search: queryString.stringify(urlParams)
        })
    }, [form])

    const optionsName = filesData.map((f) => ({label: f.fileName, value: f.fileId}));
    const optionsSender = filesData.map((f) => ({label: f.senderId.toString(), value: f.senderId}));
    const optionsChat = filesData.map((f) => ({label: f.chatId.toString(), value: f.chatId}));

    const FragmentsFiles = filesData.map((f) => <FragmentFile fileType={f.fileType} fileId={f.fileId}
                                                              fileName={f.fileName} chatId={f.chatId}
                                                              senderId={f.senderId} uploadDate={f.uploadDate}/>);
    const onChangeFileName = (e: SingleValue<FormType<number>> ) => {
        dispatch(actions.changeFilterFileName(e?.label))
    };
    const onChangeDate = (e: SingleValue<FormType<string>>) => dispatch(actions.changeFilterDate(e?.value));
    const onChangeSenders = (e: MultiValue<FormType<number>>) => dispatch(actions.changeFilterSenders(e.map((v => v?.value))));
    const onChangeChats = (e: MultiValue<FormType<number>>) => dispatch(actions.changeFilterChats(e.map((v => v?.value))));
    const onChangeCategories = (e: MultiValue<FormType<Category>>) => {
        dispatch(actions.changeFilterCategories(e.map((v => v?.value))));
    }

    return (
        <div className={"files-main"}>
            <h2 className={"files-main__title"}>Файлы</h2>
            <div className={"files-main__content"}>

                <div className={"files"}>
                    <h3 className={"files__title"}>Название</h3>
                    <h3 className={"files__title"}>Дата</h3>
                    <h3 className={"files__title"}>Формат</h3>
                    <h3 className={"files__title"}>Отправитель</h3>
                    <h3 className={"files__title"}>Чаты</h3>
                    <div className={"files__filter files__filter_select"}>
                        <Select value={optionsName.filter((e) => e.label === form.fileName)} options={optionsName}
                                styles={select} onChange={onChangeFileName} isClearable={true}/>
                    </div>
                    <div className={"files__filter files__filter_select"}>
                        <Select value={optionsDate.filter((e) => e.value === form.date)} placeholder={"дд.мм.гггг"} options={optionsDate}
                                components={{Option: CustomOption}}
                                styles={select} onChange={onChangeDate}/>
                    </div>
                    <div className={"files__filter files__filter_select"}>
                        <Select value={optionsCategory.filter((e) => form.categories?.includes(e.value))}
                                placeholder={"Выберите формат"} options={optionsCategory}
                                isMulti
                                styles={select} onChange={onChangeCategories}/>
                    </div>
                    <div className={"files__filter files__filter_select"}>
                        <Select value={optionsSender.filter((e) => form.senders?.includes(e.value))}
                                placeholder={"Выберите отправителя"} options={optionsSender}
                                isMulti styles={select}
                                onChange={onChangeSenders}
                        />
                    </div>
                    <div className={"files__filter files__filter_select"}>
                        <Select value={optionsChat.filter((e) => form.chats?.includes(e.value))}
                                placeholder={"Выберите название чата"} options={optionsChat}
                                isMulti
                                styles={select} isClearable={true} onChange={onChangeChats}/>
                    </div>
                    {FragmentsFiles}
                </div>
            </div>
            <Paginator count={filesData.length}/>
        </div>
    );
}

const CustomOption = ({innerRef, innerProps, ...props}: any) => {
    const {data} = props;
    // console.log(innerProps)
    const onClick = (e: number) => {
        alert("Жопа");
    };
    if (data.label === "Другой период...") {
        return (
            <div onClick={onClick} ref={innerRef} {...innerProps} >{data.label}</div>
        );
    }
    return (
        <div ref={innerRef} {...innerProps} >{data.label}</div>
    );
}


export default FilesMain;

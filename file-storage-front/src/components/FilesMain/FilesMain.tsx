import React, {useEffect, useState} from 'react';
import "./FilesMain.scss"
import Select, {SingleValue} from "react-select";
import Paginator from '../utils/Paginator/Paginator';
import {FormType} from "../../models/File";
import FragmentFile from "./FragmentFile";
import {useHistory} from "react-router-dom";
import * as queryString from "querystring";
import {filesSlice} from "../../redux/filesSlice";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {configFilters, EventsChange} from "./ConfigFilters";

const select = {
    input: (asd: any) => ({
        ...asd,
        height: 30,
    }),
};

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

    const {optionsName, optionsCategory, optionsSender, optionsChat, optionsDate} = configFilters(filesData);
    const {onChangeFileName,onChangeCategories, onChangeSenders, onChangeDate,
        onChangeChats} = EventsChange(dispatch, actions);

    const FragmentsFiles = filesData.map((f) => <FragmentFile fileType={f.fileType} fileId={f.fileId}
                                                              fileName={f.fileName} chatId={f.chatId}
                                                              senderId={f.senderId} uploadDate={f.uploadDate}/>);
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

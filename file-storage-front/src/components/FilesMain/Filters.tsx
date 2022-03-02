import React, {FC} from "react";
import {Select} from "../utils/Inputs/Select";
import {SelectTime} from "../utils/Inputs/SelectDate";
import {ReactComponent as Search} from "../../assets/search.svg";
import {configFilters} from "./ConfigFilters";
import {useAppSelector} from "../../utils/hooks/reduxHooks";
import {UseFormGetValues} from "react-hook-form";
import {TypeSelectFilters} from "./FilesMain";

type TypeProps = {
    setValueForm:(name: string, value: any) => void,
    getValues: UseFormGetValues<TypeSelectFilters>
}

export const Filters:FC<TypeProps> = ({setValueForm, getValues}) => {
    const filesNames = useAppSelector((state) => state.filesReducer.filesNames);
    const chats = useAppSelector((state) => state.filesReducer.chats);
    const senders = useAppSelector((state) => state.filesReducer.senders);
    const filesTypes = useAppSelector((state) => state.filesReducer.filesTypes);
    const {optionsName, optionsSender, optionsChat} = configFilters(filesNames, chats, senders);
    const optionsCategory = filesTypes && Object.keys(filesTypes).map((key) => ({label: filesTypes[key], value: key}));

    return <>
        <Select name={"fileName"} className={"files__filter files__filter_select"}
                setValue={setValueForm}
                values={getValues("fileName")} options={optionsName} isMulti={false}/>
        <SelectTime name={"date"} className={"files__filter files__filter_select"}
                    setValue={setValueForm}
                    values={getValues("date")} placeholder={"Выберите дату"}/>
        <Select name={"categories"} className={"files__filter files__filter_select"}
                setValue={setValueForm}
                values={getValues("categories")} options={optionsCategory} isMulti={true}/>
        <Select name={"senderIds"} className={"files__filter files__filter_select"}
                setValue={setValueForm}
                values={getValues("senderIds")} options={optionsSender} isMulti={true}/>
        <div className={"files__filter files__filter_last files__filter_select files__filter_search"}>
            <Select name={"chatIds"}
                    setValue={setValueForm}
                    values={getValues("chatIds")} options={optionsChat} isMulti={true}/>
            <button>
                <Search/>
            </button>
        </div>
    </>
}

import React from 'react';
import "./Messages.scss";
import {MessageType, MessageTypeEnum} from "../../../models/File";
import {useAppDispatch} from "../../../utils/hooks/reduxHooks";
import {profileSlice} from "../../../redux/profileSlice";
import error from "../../../assets/error.svg";
import success from "../../../assets/success.svg";

const {clearMessage} = profileSlice.actions;
export const Messages: React.FC<{ className?: string, messages: Array<MessageType> }> = ({messages, className}) => {
    const messagesUi = messages.map(({value, type}, index) => <Message key={index} type={type} value={value}
                                                                       index={index}/>);

    return (
        <section className={(`${className} ` ?? "") + "messages"}>
            {messagesUi}
        </section>
    )
}

const Message: React.FC<{ value: string, type: MessageTypeEnum, index: Number }> = ({value, type, index}) => {
    const dispatch = useAppDispatch();
    const dict = {
        [MessageTypeEnum.Error]: {
            text: "Проблема",
            icon: null,
        },
        [MessageTypeEnum.Message]: {
            text: "Успешно",
            icon: null,
        }
    }

    function onClose() {
        dispatch(clearMessage(index));
    }

    if (dict[type].text == "Успешно") {
        return (
            <div className={"message color-success"}>
                <img src={success}/>
                <h2>
                    {dict[type].text}
                </h2>
                <p className={"p-padding"}>{value}</p>
                <button className={"close-button"} onClick={onClose}/>
            </div>
        )
    } else {
        return (
            <div className={"message color-error"}>
                <img src={error}/>
                <h2>
                    {dict[type].text}
                </h2>
                <p className={"p-padding"}>{value}</p>
                <button className={"close-button"} onClick={onClose}/>
            </div>
        )
    }
}




import React, {useState} from 'react';
import "./Select.scss";
import {OutsideAlerter} from "../OutSideAlerter/OutSideAlerter";

const optionsDate = [
    {value: 'За все время', label: 'За все время'},
    {value: 'Сегодня', label: 'Сегодня'},
    {value: 'Вчера', label: 'Вчера'},
    {value: 'За последние 7 дней', label: 'За последние 7 дней'},
    {value: 'За последние 30 дней', label: 'За последние 30 дней'},
    {value: null, label: 'Другой период...'}
];

type PropsDate = {
    name: string, onChangeForm: any,
    className?: string, register: any, values: any, setValue: any,
    placeholder?: string, isMulti?: boolean
};

export const SelectTime: React.FC<PropsDate> = ({
                                                    name,
                                                    onChangeForm,
                                                    className,
                                                    register,
                                                    values,
                                                    setValue,
                                                    placeholder,
                                                }) => {
    if (!placeholder)
        placeholder = "Введите текст";
    const [isOpen, changeOpen] = useState(false);
    const [text, changeText] = useState("");
    const [strangeDate, changeStrangeDate] = useState(false);
    const regexp = new RegExp(`${text}`, 'i');
    const ui = optionsDate.filter((elem) => !!elem.label.match(regexp))
        .map((elem) => {
            const onChange = () => {
                if (!elem.value) {
                    changeStrangeDate(true);
                } else {
                    setValue(name, elem.value)
                    onChangeForm();
                }
            }

            return <li key={elem.value}
                       className={"select__option " + (values && (values.includes(elem.value)) ? "select__option_active" : "")}
                       onClick={onChange}>{elem.label}</li>;
        })

    return (
        <OutsideAlerter className={className} onOutsideClick={() => {
            changeOpen(false);
        }}>
            <div className={"select"}>
                <input className={"select__field"} onClick={() => changeOpen(!isOpen)} value={text}
                       onChange={(e) => {
                           changeText(e.target.value)
                       }} placeholder={placeholder}/>
                <ul className={"select__list " + (isOpen ? "select__list_open" : "")}>
                    <div className="select__scroll">
                        {strangeDate ? <FormTime
                                setValue={(value: any) => {
                                    setValue(name, value);
                                    onChangeForm();
                                }}
                                onDecline={() => changeStrangeDate(false)}/>
                            : ui}
                    </div>
                </ul>
                <select multiple={true} {...register(name, {onChange: () => onChangeForm})}/>
            </div>
        </OutsideAlerter>
    )
}

export const FormTime = ({className, setValue, onDecline}: any) => {
    const [date, changeDate] = useState({start: null as null | string, end: null as null | string})
    const onAccept = (e: any) => {
        e.preventDefault();
        setValue(`${date.start} ${date.end}`);
        onDecline();
    }
    return <div className={className ? className + " input-date" : "input-date"}>
        <div className={"input-date__label"}>От</div>
        <input type={"date"} onChange={(e) => {
            changeDate({...date, start: e.target.value});
        }}/>
        <div className={"input-date__label"}>До</div>
        <input type={"date"} onChange={(e) => {
            changeDate({...date, end: e.target.value});
        }}/>
        <div className={"input-date__btns"}>
            <button className={"input-date__accept"} onClick={onAccept}>Применить</button>
            <button className={"input-date__back"} onClick={onDecline}>Назад</button>
        </div>
    </div>
}

import React, {useState} from 'react';
import "./Select.scss";
import {OutsideAlerter} from "../OutSideAlerter/OutSideAlerter";

function createDateISO(date:Date | number){
    const date1 = new Date(date);
    date1.setHours(0,0,0,0);
    return date1.toISOString();
}

const optionsDate = [
    {
        value: {
            dateTo: null,
            dateFrom: null
        }, label: 'За все время'
    },
    {
        value: {
            // dateTo: createDateISO(new Date().setDate(new Date().getDate() + 1)),
            dateTo: null,
            dateFrom: createDateISO(new Date()),
        },
        label: 'Сегодня'
    },
    {
        value: {
            dateTo: createDateISO(new Date()),
            dateFrom: createDateISO(new Date().setDate(new Date().getDate() - 1))
        }, label: 'Вчера'
    },
    {
        value: {
            dateTo: null,
            dateFrom: createDateISO(new Date().setDate(new Date().getDate() - 6))
        }, label: 'За последние 7 дней'
    },
    {
        value: {
            dateTo: null,
            dateFrom: createDateISO(new Date().setDate(new Date().getDate() - 29))
        }, label: 'За последние 30 дней'
    },
    {value: null, label: 'Другой период...'}
];

type PropsDate = {
    name: string,
    className?: string,values: any, setValue: (name: string, value: any) => void,
    placeholder?: string, isMulti?: boolean
};

export const SelectTime: React.FC<PropsDate> = ({
                                                    name,
                                                    className,
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
                }
            }


            //todo: refactor
            function className() {
                if (!values)
                    return ""
                const f = !optionsDate.find((elem) =>
                    ((values.dateTo === elem.value?.dateTo && values.dateFrom === elem.value?.dateFrom)));
                if (f && elem.label === "Другой период...") {
                    return "select__option_active";
                }

                return (values.dateTo === elem.value?.dateTo && values.dateFrom === elem.value?.dateFrom) ? "select__option_active" : ""
            }

            return <li key={JSON.stringify(elem.value)}
                       className={"select__option " + className()}
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
                       }} placeholder={calcPlaceholder(values, optionsDate) ?? placeholder}/>
                <ul className={"select__list " + (isOpen ? "select__list_open" : "")}>
                    <div className="select__scroll">
                        {strangeDate ? <FormTime
                                setValue={(value: any) => {
                                    setValue(name, value);
                                }}
                                onDecline={() => changeStrangeDate(false)}/>
                            : ui}
                    </div>
                </ul>
            </div>
        </OutsideAlerter>
    )
};


const calcPlaceholder = (value: any | null, options: Array<{ value: any, label: string }>) => {
    const label = options?.find(e => value?.dateTo === e.value?.dateTo && value?.dateFrom === e.value?.dateFrom)?.label;
    return label ?? "Другой период";
}

export const FormTime = ({className, setValue, onDecline}: any) => {
    const [date, changeDate] = useState({start: null as null | string, end: null as null | string})
    const onAccept = (e: any) => {
        e.preventDefault();
        if (date.end && date.start) {
            setValue({
                dateTo: new Date(date.end).toISOString(),
                dateFrom: new Date(date.start).toISOString()
            });

            onDecline();
        }
    };

    return <div className={className ? className + " input-date" : "input-date"}>
        <div className={"input-date__label"}>От</div>
        <input className={"input-date__input"} type={"date"} onChange={(e) => {
            changeDate({...date, start: e.target.value});
        }}/>
        <div className={"input-date__label"}>До</div>
        <input className={"input-date__input"} type={"date"} onChange={(e) => {
            changeDate({...date, end: e.target.value});
        }}/>
        <div className={"input-date__btns"}>
            <button className={"input-date__accept"} onClick={onAccept}>Применить</button>
            <button className={"input-date__back"} onClick={onDecline}>Назад</button>
        </div>
    </div>
}

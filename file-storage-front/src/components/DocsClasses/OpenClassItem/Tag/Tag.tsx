import React, {ChangeEvent, FC, memo, useEffect, useRef, useState} from 'react';
import classes from "./Tag.module.scss";
import {ReactComponent as Cross} from "./../../../../assets/cross.svg";
import {Button} from "../../../utils/Button/Button";

type PropsType = {}


const Tag: FC<PropsType> = memo(() => {
    const start = "text"
    const input = useRef<HTMLInputElement>(null);
    const [text, setText] = useState(start);

    useEffect(() => {
        if (input.current) {
            input.current.style.width = "0";
            input.current.style.width = input.current.scrollWidth + "px";
        }
    }, [input.current])

    function onChange(e: ChangeEvent<HTMLInputElement>) {
        if (!input.current)
            return;
        setText(e.target.value);
        input.current.style.width = "0";
        input.current.style.width = Math.max(input.current.scrollWidth, 10) + "px";
    }

    return <div className={classes.tag}>
        <input ref={input} value={text} onChange={onChange} className={classes.input}/>
        <Cross className={classes.cross}/>
    </div>;
});

export const CreateTag: FC = () => {
    const [editMode, setEditMode] = useState(false);
    return (!editMode ?
        <Button onClick={() => setEditMode(true)} className={classes.createBtn} type={"white"}>Добавить</Button> :
        <TagCreate callback={() => setEditMode(false)}/>);
}


type PropsTagCreateType = {
    callback: () => void
}

export const TagCreate: FC<PropsTagCreateType> = memo(({callback}) => {
    const input = useRef<HTMLInputElement>(null);
    const [text, setText] = useState("");

    useEffect(() => {
        if (input.current) {
            input.current.focus();
            input.current.style.width = "0";
            input.current.style.width = 10 + "px";
        }
    }, [input.current])

    function onChange(e: ChangeEvent<HTMLInputElement>) {
        if (!input.current)
            return;
        setText(e.target.value);
        input.current.style.width = "0";
        input.current.style.width = Math.max(input.current.scrollWidth, 10) + "px";
    }

    return <div className={classes.tag}>
        <input ref={input} value={text} onChange={onChange} className={classes.input} onBlur={callback}/>
    </div>;
});

export default Tag;






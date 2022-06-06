import React, {ChangeEvent, FC, KeyboardEvent, memo, useEffect, useRef, useState} from 'react';
import classes from "./Tag.module.scss";
import {ReactComponent as Cross} from "./../../../../assets/cross.svg";
import {Button} from "../../../utils/Button/Button";
import {ClassificationType} from "../../../../models/Classification";

type PropsType = {
    tag: NonNullable<ClassificationType["classificationWords"]>[0],
    removeTag: () => void
}


const Tag: FC<PropsType> = memo(({tag, removeTag}) => {
    return <div className={classes.tag}>
        <div className={classes.input}>{tag.value}</div>
        <Cross className={classes.cross} onClick={removeTag}/>
    </div>;
});

export const CreateTag: FC<{ setTag: (value: string) => void }> = ({setTag}) => {
    const [editMode, setEditMode] = useState(false);

    return (!editMode ?
        <Button onClick={() => setEditMode(true)} className={classes.createBtn} type={"white"}>Добавить</Button> :
        <TagCreate close={() => setEditMode(false)} setTag={setTag}/>);
}


type PropsTagCreateType = {
    close: () => void,
    setTag: (e: string) => void
}

export const TagCreate: FC<PropsTagCreateType> = memo(({close, setTag}) => {
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

    function onEnter(e: KeyboardEvent<HTMLInputElement>) {
        if (input.current && e.key === "Enter") {
            input.current.blur()
        }
    }

    function onBlur() {
        if (text.length > 0)
            setTag(text);
        close();
    }

    return <div className={classes.tag}>
        <input onKeyPress={onEnter} ref={input} value={text} onChange={onChange} className={classes.input}
               onBlur={onBlur}/>
    </div>;
});

export default Tag;

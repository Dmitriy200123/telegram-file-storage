import React, {FC, memo, useEffect, useRef, useState} from "react";
import {Button} from "../../../../utils/Button/Button";
import classes from "./Tags.module.scss";

export const Tags: FC<PropsControlType> = memo(({}) => {
    const [isOpen, setIsOpen] = useState(false);
    const div = useRef<HTMLDivElement>(null);
    const [isOverflow, setIsOverflow] = useState(false);
    useEffect(() => {
        if (!div.current) {
            return;
        }

        setIsOverflow((div.current.scrollHeight > 30));
    }, [div.current])
    return <div className={classes.tags}>
        <div className={classes.tags__items + (isOpen ? ` ${classes.tags__items_active}` : "")} ref={div}>
            <div className={classes.tags__item}><span>техническое</span></div>
            <div className={classes.tags__item}><span>техническое</span></div>
            <div className={classes.tags__item}><span>техническое</span></div>
            <div className={classes.tags__item}><span>техническое</span></div>
            <div className={classes.tags__item}><span>техническое</span></div>
            <div className={classes.tags__item}><span>техническое</span></div>
        </div>
        {isOverflow && <Button className={classes.tags__btn} onClick={() => setIsOpen((prev) => !prev)}>
            {isOpen ? "Свернуть" : "Ещё"}
        </Button>}
    </div>
});

type PropsControlType = {};

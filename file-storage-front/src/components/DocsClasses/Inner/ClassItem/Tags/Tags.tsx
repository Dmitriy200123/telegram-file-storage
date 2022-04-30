import React, {FC, memo, useEffect, useRef, useState} from "react";
import {Button} from "../../../../utils/Button/Button";
import classes from "./Tags.module.scss";
import {ClassificationType} from "../../../../../models/Classification";

type PropsControlType = {
    tags: ClassificationType["classificationWords"]
}

export const Tags: FC<PropsControlType> = memo(({tags}) => {
    const [isOpen, setIsOpen] = useState(false);
    const div = useRef<HTMLDivElement>(null);
    const [isOverflow, setIsOverflow] = useState(false);

    function onChangeSize(){
        if (!div.current) {
            return;
        }
        setIsOverflow((div.current.scrollHeight > 30));
    }

    useEffect(() => {
        window.addEventListener("resize", onChangeSize);
        return () => window.removeEventListener("resize", onChangeSize);

    }, []);

    useEffect(() => {
        onChangeSize()
    }, [div.current])

    return <div className={classes.tags}>
        <div className={classes.tags__items + (isOpen ? ` ${classes.tags__items_active}` : "")} ref={div}>
            {tags?.map((t) => {
                return <div key={t.id} className={classes.tags__item}><span>{t.value}</span></div>
            })}
        </div>
        {isOverflow && <Button className={classes.tags__btn} onClick={() => setIsOpen((prev) => !prev)}>
            {isOpen ? "Свернуть" : "Ещё"}
        </Button>}
    </div>
});


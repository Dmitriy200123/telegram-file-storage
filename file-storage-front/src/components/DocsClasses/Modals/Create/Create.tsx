import React, {ChangeEvent, FC, memo, useEffect, useRef, useState} from 'react';
import classes from "./../DocsClassesModal/DocsClassesModal.module.scss";
import {InputText} from "../../../utils/Inputs/Text/InputText";
import {Button} from "../../../utils/Button/Button";
import {useAppDispatch} from "../../../../utils/hooks/reduxHooks";
import {postAddClassification} from "../../../../redux/classesDocs/classesDocsThunks";
import classNames from 'classnames';

type Props = {
    onOutsideClick?: () => void
}

const Create: FC<Props> = ({onOutsideClick}) => {
    const dispatch = useAppDispatch();
    const [array, setArray] = useState<{ value: string }[]>([]);
    const [tag, setTag] = useState<string>("");
    const [name, setName] = useState<string>("");

    function saveTag() {
        if (tag.length > 0 && !array.find((e) => e.value === tag)) {
            setArray([...array, {value: tag}]);
            setTag("");
        }
    }

    function onChangeTag(e: ChangeEvent<HTMLInputElement>) {
        setTag(e.target.value);
    }

    function onChangeName(e: ChangeEvent<HTMLInputElement>) {
        setName(e.target.value);
    }

    function onSubmit() {
        if (name.length === 0)
            return;
        dispatch(postAddClassification({name: name, classificationWords: array}));
    }

    return (
        <div className={classes.wrapper}>
            <h2 className={classes.h2}>Создание классификации документов</h2>
            <div className={classes.filters}>
                <div className={classes.filter}>
                    <label className={classes.label}>Название</label>
                    <InputText placeholder={"Введите название классификации"} onChange={onChangeName} value={name}/>
                </div>
                <div className={[classes.filter, classes.filter_class].join(" ")}>
                    <label className={classes.label}>Слова для классификации</label>
                    <div className={classes.btnAddWrapper}>
                        <InputText placeholder={"Введите слово для классификации"} onChange={onChangeTag} value={tag}/>
                        <button onClick={saveTag} className={classes.btnAdd}>{">"}</button>
                    </div>
                    <Tags array={array}/>
                </div>
            </div>

            <div className={classes.btns}>
                <Button onClick={onSubmit} disabled={name.length === 0}>ОК</Button>
                <Button type={"transparent"} onClick={onOutsideClick}>ОТМЕНА</Button>
            </div>
        </div>
    );
};


const Tags: FC<{ array: { value: string }[] }> = memo(({array}) => {
    const wrapper = useRef<HTMLDivElement>(null);
    const [isMany, setIsMany] = useState(false);
    const [isReveal, setIsReveal] = useState(false);

    useEffect(() => {
        if (wrapper.current && wrapper.current.scrollHeight > 70 ) {
            setIsMany(true);
        } else {
            setIsMany(false);
            setIsReveal(false);
        }
    }, [array])

    return <div className={classes.tags}>
        <div className={classNames(classes.tagsWrapper, isReveal && classes.tagsWrapper_open)} ref={wrapper}>
            {array.map(e => {
                return <div key={e.value} className={classes.tag}><span>{e.value}</span></div>
            })}
        </div>
        {isMany && <button className={classes.tagsBtn} onClick={() => setIsReveal((prev) => !prev)}>Показать все</button>}
    </div>
})

export default Create;

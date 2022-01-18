import React, {memo} from "react";
import "./LoadText.scss"
import {Select} from "../../utils/Inputs/Select";
import {SubmitHandler, useForm} from "react-hook-form";
import {Button} from "../../utils/Button/Button";
import {Link} from "react-router-dom";
import {InputText} from "../../utils/Inputs/Text/InputText";
import {useAppSelector} from "../../../utils/hooks/reduxHooks";
import {postCustomFile} from "../../../redux/thunks/fileThunks";
import {AppDispatch} from "../../../redux/redux-store";
import {useDispatch} from "react-redux";

export const LoadText: React.FC<{ dispatch: AppDispatch, className?: string }> = memo(({className}) => {
    const {register, handleSubmit, formState: {errors}, setValue, getValues, reset} = useForm();
    const dispatch = useDispatch();

    const {filesTypes} = useAppSelector(state => state.filesReducer);
    const optionsCategory = filesTypes && Object.keys(filesTypes).filter(key => key === "4" || key === "5")
        .map((key) => ({label: filesTypes[key], value: key}));

    const dispatchValuesForm: SubmitHandler<{ contentType: string, FileName: string, message: string }> = (formData) => {
        if (formData.contentType) {
            dispatch(postCustomFile(formData));
            reset({contentType: null, FileName: null, message: null});
        }
    };


    const onChangeForm = handleSubmit(dispatchValuesForm);
    const setValueForm = (name: any, value: any) => {
        setValue(name, value, {
            shouldValidate: true,
            shouldDirty: true
        });
    }


    return (
        <div className={className + "  load-text"}>
            <form onSubmit={onChangeForm} className={"load-text__form"}>
                <div className={"load-text__form-inputes"}>
                    <div>
                        <p>Тип Файла</p>
                        <Select name={"contentType"} register={register}
                                onChangeForm={onChangeForm} setValue={setValueForm}
                                values={getValues("contentType")} options={optionsCategory} isMulti={false}
                                placeholder={""}/>
                    </div>
                    <label>
                        <p>Название</p>
                        <InputText type={"text"} form={{...register("FileName",{ required: true })}}/>
                    </label>
                    <label>
                        <p>Ссылка или текст</p>
                        <InputText type={"text"} form={{...register("message", { required: true })}}/>
                    </label>
                </div>
                <div className={"load-text__btns"}>
                    <Button>Загрузить</Button>
                    <Link to={"/load/"}><Button type={"transparent"}>Отмена</Button></Link>
                </div>
            </form>
        </div>

    );
})





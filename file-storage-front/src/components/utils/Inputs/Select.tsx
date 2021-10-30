import React from 'react';
import {useForm} from "react-hook-form";

const Select: React.FC = () => {
    const options = {};
    const name = "123";
    const {register, handleSubmit, watch, formState: {errors}, setValue} = useForm();
    watch("example")
    const select = watch("example",'value');
    const onchange = (e: any) => {
        console.log(e)
    }

    return (
        <form onChange={handleSubmit(onchange)} onSubmit={handleSubmit(onchange)}>
            <div style={{padding: 100}} className={"select"}>
                <div onClick={() => setValue("example",["1"])}>Клик меня</div>
                <select multiple={true} {...register("example")}>
                    <option value={1}>1111</option>
                    <option value={2}>222</option>
                </select>
            </div>
            <button type={"submit"}>asd</button>
        </form>
    )
}

export default Select;

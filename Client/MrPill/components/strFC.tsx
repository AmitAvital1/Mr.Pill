import { Text } from "react-native"

// converts string to react simple react text Function Component
export const strFC = (str: string, size?: number) => {
    const words = str.split(" ")
    let result = []
    return () => {
        for(let i = 0; i < words.length; i++)
            result.push(<Text style={{fontSize: size? size: 26, fontWeight: 'bold',}}>{words[i]}</Text>)
        return result;
    };
};
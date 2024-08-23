import { Text, StyleSheet } from "react-native"

// converts string to simple react function component for display of words in a column
export const strFC = (str: string, size?: number) => {
    const words = str.split(" ")
    let result = []

    size = size ? size : 26;
    const topGap = words.length == 1? 50 : 0;
    const bottomGap = words.length == 3? 10 : 5;

    return () => {

        for(let i = 0; i < words.length; i++) {
            result.push(
                <Text style={[styles.text, {fontSize: size, paddingBottom: i == words.length - 1? 30 + topGap - 2 * bottomGap: 0, paddingTop: bottomGap}]}>{words[i]}</Text>,
            )
        }

        return result;
    };
};

const styles = StyleSheet.create({
    text: {
        color: "#FFF",
        fontWeight: 'bold',
        textShadowColor: 'rgba(0,0,0,1)', // Maximum opacity
        textShadowRadius: 3,
        textShadowOffset: { 
            width: 1, // Slight offset to the right
            height: 1  // Slight offset to the bottom
        }
    },
    textStroke: {
        color: "#FFF",
        fontWeight: 'bold',
        textShadowColor: 'rgba(0,0,0,1)', 
        textShadowRadius: 3,
        textShadowOffset: { 
            width: -1, // Slight offset to the left
            height: -1 // Slight offset to the top
        }
    }
});
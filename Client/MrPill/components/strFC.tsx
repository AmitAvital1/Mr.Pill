import { Text, StyleSheet, View } from "react-native"

// converts string to simple react JSX element for display of words in a column
export const strFC = (str: string, size?: number) => {
    const words = str.split(" ")
    size = size ? size : 26;
    
    function renderWord(word: string, index: number) {
        return <Text key={index} style={[styles.text, {fontSize: size}]}>{word}</Text>
    }
    return <View style={{gap: 10, alignContent: 'center', justifyContent: 'center',}}>{words.map((word, index) => renderWord(word, index))}</View>
};

const styles = StyleSheet.create({
    text: {
        textAlign: 'center',
        elevation: 3,
        color: "#FFF",
        fontWeight: 'bold',
        textShadowColor: 'rgba(0,0,0,1)', // Maximum opacity
        textShadowRadius: 3,
        textShadowOffset: { 
            width: 1, // Slight offset to the right
            height: 1  // Slight offset to the bottom
        }
    },
});
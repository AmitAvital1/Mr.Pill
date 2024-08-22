import type { PropsWithChildren, ReactElement } from 'react';
import { StyleSheet, useColorScheme, View } from 'react-native';
import Animated, {
  interpolate,
  useAnimatedRef,
  useAnimatedStyle,
  useScrollViewOffset,
} from 'react-native-reanimated';

import { ThemedView } from '@/components/ThemedView';

type Props = PropsWithChildren<{
  InsideColor: string,
  OutsideColor: string
}>;



function Gradient(insideColor: string, outsideColor: string) {

    const insideRed = insideColor.substring(1,3);
    const insideGreen = insideColor.substring(3,5);
    const insideBlue = insideColor.substring(5,7);
    let outsideRed = outsideColor.substring(1,3);
    let outsideGreen = outsideColor.substring(3,5);
    let outsideBlue = outsideColor.substring(5,7);

    let result = ""

    function GradientRecursion() {
        if (insideRed == outsideRed && insideGreen == outsideGreen && insideBlue == outsideBlue) {
            return;
        }
        result += "<View style={{backgroundColor: #" + outsideRed + outsideGreen + outsideBlue;
        //outsideRed = Math.abs()
    }

    return <div className="Container" dangerouslySetInnerHTML={{__html: result}}></div>;
}

export default function GradientView({
  children,
  InsideColor,
  OutsideColor,
}: Props) {

  const colorScheme = useColorScheme() ?? 'light';
  const scrollRef = useAnimatedRef<Animated.ScrollView>();
  const scrollOffset = useScrollViewOffset(scrollRef);

  return (
    <ThemedView style={{flex: 1, backgroundColor: OutsideColor}}>
        <ThemedView style={[styles.content, {backgroundColor: InsideColor}]}>{children}</ThemedView>
    </ThemedView>
  );
}


const styles = StyleSheet.create({
  content: {
    flex: 1,
    padding: 5,
    gap: 3,
    overflow: 'hidden',
  },
});

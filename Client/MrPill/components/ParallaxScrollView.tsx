import React, { forwardRef, PropsWithChildren, ReactElement, useImperativeHandle, useRef } from 'react';
import { StyleSheet, useColorScheme, LayoutChangeEvent, Button } from 'react-native';
import Animated, {
  interpolate,
  useAnimatedRef,
  useAnimatedStyle,
  useScrollViewOffset,
  runOnUI,
  scrollTo,
} from 'react-native-reanimated';

import { ThemedView } from '@/components/ThemedView';

type Props = PropsWithChildren<{
  backgroundColor?: string;
  headerBackgroundColor?: { dark: string; light: string };
  headerImage?: ReactElement;
  headerHeight?: number;
}>;

const ParallaxScrollView = forwardRef(function ParallaxScrollView(
  { children, headerImage, headerBackgroundColor, backgroundColor, headerHeight }: Props,
  ref
) {
  backgroundColor = backgroundColor || "#FFF";
  if (!headerBackgroundColor) headerBackgroundColor = { dark: backgroundColor, light: backgroundColor };
  headerHeight = headerHeight ? headerHeight : headerImage ? 250 : 0;
  if (!headerImage) headerImage = <></>;

  const colorScheme = useColorScheme() ?? 'light';
  const scrollRef = useAnimatedRef<Animated.ScrollView>();
  const childPositions = useRef<number[]>([]).current;
  const scrollOffset = useScrollViewOffset(scrollRef);

  const headerAnimatedStyle = useAnimatedStyle(() => {
    return {
      transform: [
        {
          translateY: interpolate(
            scrollOffset.value,
            [-headerHeight, 0, headerHeight],
            [-headerHeight / 2, 0, headerHeight * 0.75]
          ),
        },
        {
          scale: interpolate(scrollOffset.value, [-headerHeight, 0, headerHeight], [2, 1, 1]),
        },
      ],
    };
  });

  const handleLayout = (event: LayoutChangeEvent, index: number) => {
    const { height } = event.nativeEvent.layout;
    childPositions[index] = (childPositions[index - 1] || 0) + height;
  };

  const scrollToChild = (index: number) => {
    const offset = childPositions[index] || 0;
    runOnUI(() => {
      scrollTo(scrollRef, 0, offset, true);
    })();
  };

  useImperativeHandle(ref, () => ({
    scrollToChild,
  }));

  return (
    <ThemedView style={{ flex: 1, backgroundColor: backgroundColor }}>
      <Animated.ScrollView ref={scrollRef} scrollEventThrottle={16} keyboardShouldPersistTaps='always'>
        <Animated.View
          style={[
            { height: headerHeight, overflow: 'hidden' },
            { backgroundColor: headerBackgroundColor[colorScheme] },
            headerAnimatedStyle,
          ]}
        >
          {headerImage}
        </Animated.View>
        <ThemedView style={[styles.content, { backgroundColor: backgroundColor }]}>
          {React.Children.map(children, (child, index) => (
            <Animated.View key={index} onLayout={(event) => handleLayout(event, index)}>
              {child}
            </Animated.View>
          ))}
        </ThemedView>
      </Animated.ScrollView>
    </ThemedView>
  );
});

const styles = StyleSheet.create({
  content: {
    flex: 1,
    padding: 5,
    gap: 3,
    overflow: 'hidden',
  },
});

export default ParallaxScrollView;
